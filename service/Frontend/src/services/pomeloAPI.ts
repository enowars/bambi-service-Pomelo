export interface EmployeeProjectHours {
  employeeId: number,
  projectId: number,
  totalHours: number,
  deliveredHours: number
}

export interface EmployeeProjectWeeklyCapacityDto {
  employeeId: number,
  projectId: number,
  start: string,
  capacity: number
}

export interface EmployeeDto {
  id: number,
  name: string,
  department: string,
  note: string | null,
}

export interface EmployeeDetails {
  id: number,
  name: string,
  department: string,
  note: string | null,
  employeeProjectHours: EmployeeProjectHours[],
  employeeProjectWeeklyCapacities: EmployeeProjectWeeklyCapacityDto[]
}

export interface Project {
  id: number,
  name: string,
  begin: string,
  end: string,
  employeeProjectHours: EmployeeProjectHours[],
  employeeProjectWeeklyCapacities: EmployeeProjectWeeklyCapacityDto[],
  deliveredHoursTimestamp: string
}

// Account Controller
export const getAccount = async () : Promise<EmployeeDto | null> => {
  try {
    return await (await fetch('/api/account/account')).json() as EmployeeDto
  } catch {
    return null
  }
}

export const getEmployee = async (employeeId: number) : Promise<EmployeeDetails> => {
  return await fetch('/api/account/employee?employeeId=' + employeeId, {
    method: 'GET'
  }).then(val => val.json()) as EmployeeDetails
}

export const getEmployees = async () : Promise<EmployeeDto[]> => {
  return await fetch('/api/account/employees', {
    method: 'GET'
  }).then(val => val.json()) as EmployeeDto[]
}

export const getAccountDepartment = async () : Promise<EmployeeDetails[]> => {
  return await fetch('/api/account/employees', {
    method: 'GET'
  }).then(val => val.json()) as EmployeeDetails[]
}

export const postRegister = async (employeeName: string, department: string, note: string | null) : Promise<EmployeeDetails> => {
  const form = new FormData()
  form.append('employeeName', employeeName)
  form.append('department', department)
  if (note != null) {
    form.append('note', note)
  }
  return await fetch('/api/account/register', {
    method: 'POST',
    body: form
  }).then(val => val.json()) as EmployeeDetails
}

// Booking Controller

// Project Controller
export const getProjectDepartmentProjects = async () : Promise<Project[]> => {
  return await fetch('/api/project/projects', {
    method: 'GET'
  }).then(val => val.json()) as Project[]
}

export const getProjectDetails = async (projectId: number) : Promise<Project> => {
  return await fetch('/api/project/project?projectId=' + projectId, {
    method: 'GET'
  }).then(val => val.json()) as Project
}

export const postProjectCreate = async (projectName: string, begin: Date, end: Date) : Promise<Project | null> => {
  const form = new FormData()
  form.append('name', projectName)
  form.append('begin', begin.toUTCString())
  form.append('end', end.toUTCString())
  try {
    return await fetch('/api/project/project', {
      method: 'POST',
      body: form
    }).then(val => val.json()) as Project
  } catch {
    return null
  }
}

export const postProjectTotalPlanning = async (employeeId: number, projectId: number, hours: number) : Promise<EmployeeProjectHours | null> => {
  console.log(employeeId)
  console.log(projectId)
  console.log(hours)
  const form = new FormData()
  form.append('employeeId', employeeId.toString())
  form.append('projectId', projectId.toString())
  form.append('hours', hours.toString())
  try {
    return await fetch('/api/project/hours', {
      method: 'POST',
      body: form
    }).then(val => val.json()) as EmployeeProjectHours
  } catch {
    return null
  }
}

export const postWeeklyProjectCapacity = async (employeeId: number, projectId: number, start: string, capacity: number) : Promise<void> => {
  const form = new FormData()
  form.append('employeeId', employeeId.toString())
  form.append('projectId', projectId.toString())
  form.append('start', start)
  form.append('capacity', capacity.toString())
  try {
    await fetch('/api/project/capacity', {
      method: 'POST',
      body: form
    })
  } catch {
    console.log('postWeeklyProjectCapacity failed')
  }
}
