export interface PlannedHours {
  id: number,
  employeeId: number,
  projectId: number,
  totalHours: number,
  performedHours: number
}

export interface WeeklyProjectCapacity {
  id: number,
  employeeId: number,
  projectId: number,
  start: string,
  hours: number
}

export interface Employee {
  id: number,
  name: string,
  department: string,
  note: string | null,
  plannedHours: PlannedHours[],
  weeklyProjectCapacities: WeeklyProjectCapacity[]
}

export interface Project {
  id: number,
  name: string,
  begin: string,
  end: string,
  plannedHours: PlannedHours[],
  weeklyProjectCapacities: WeeklyProjectCapacity[]
}

export const getAccountInfo = async () : Promise<Employee | null> => {
  try {
    return await (await fetch('/api/account/info')).json() as Employee
  } catch {
    return null
  }
}

export const getAccountData = async () : Promise<Employee | null> => {
  try {
    return await (await fetch('/api/account/data')).json() as Employee
  } catch {
    return null
  }
}

export const postAccountRegister = async (employeeName: string, department: string) : Promise<Employee> => {
  const form = new FormData()
  form.append('employeeName', employeeName)
  form.append('department', department)
  return await fetch('/api/account/register', {
    method: 'POST',
    body: form
  }).then(val => val.json()) as Employee
}

export const postProjectCreate = async (projectName: string, begin: Date, end: Date) : Promise<Project | null> => {
  const form = new FormData()
  form.append('name', projectName)
  form.append('begin', begin.toUTCString())
  form.append('end', end.toUTCString())
  try {
    return await fetch('/api/project/create', {
      method: 'POST',
      body: form
    }).then(val => val.json()) as Project
  } catch {
    return null
  }
}

export const getProjectDepartmentProjects = async () : Promise<Project[]> => {
  return await fetch('/api/project/departmentprojects', {
    method: 'GET'
  }).then(val => val.json()) as Project[]
}

export const getProjectDetails = async (projectId: number) : Promise<Project> => {
  return await fetch('/api/project/details?projectId=' + projectId, {
    method: 'GET'
  }).then(val => val.json()) as Project
}

export const getAccountDepartment = async () : Promise<Employee[]> => {
  return await fetch('/api/account/department', {
    method: 'GET'
  }).then(val => val.json()) as Employee[]
}

export const postProjectTotalPlanning = async (employeeId: number, projectId: number, hours: number) : Promise<PlannedHours | null> => {
  console.log(employeeId)
  console.log(projectId)
  console.log(hours)
  const form = new FormData()
  form.append('employeeId', employeeId.toString())
  form.append('projectId', projectId.toString())
  form.append('hours', hours.toString())
  try {
    return await fetch('/api/project/totalplanning', {
      method: 'POST',
      body: form
    }).then(val => val.json()) as PlannedHours
  } catch {
    return null
  }
}

export const getAccountUserData = async (employeeId: number) : Promise<Employee> => {
  return await fetch('/api/account/userdata?employeeId=' + employeeId, {
    method: 'GET'
  }).then(val => val.json()) as Employee
}

export const postWeeklyProjectCapacity = async (employeeId: number, projectId: number, start: string, capacity: number) : Promise<void> => {
  const form = new FormData()
  form.append('employeeId', employeeId.toString())
  form.append('projectId', projectId.toString())
  form.append('start', start)
  form.append('capacity', capacity.toString())
  try {
    await fetch('/api/project/weeklyprojectcapacity', {
      method: 'POST',
      body: form
    })
  } catch {
    console.log('postWeeklyProjectCapacity failed')
  }
}
