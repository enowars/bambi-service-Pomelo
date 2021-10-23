# Pomelo
planned hours
planned capacity

## Features
- login
- create department
- add user to department/join department?
- create department-specific project
- see projects of department (and their deviation)
- see employee summary
- plan capacity for a week (with an optional comment)
- personal page (see all projects with bookings)
- public/private projects?

## DB
Users(uuid)
Planning(userId, projectId, amount)

## Storage
- /data/Pomelo.db
- /data/departments/$department_uuid/projects/$project_uuid/reports/$report_id
- /data/projects/$project_uuid/reports/$report_id

## API
See swagger

### Mounts
- "./data:/data"
- cookie signing key


## Flagstores
- 0: User.Note
- 1: Project.Name
- 2: Booking file header

## Vulns
- 0: Register with same username and update note?
    - pwns: 0
- 1: Create TotalPlanning/Capacity in foreign project
    - pwns: 1
- 2: new Guid() is null-y guid, get booking file
    - pwns: 2

### Add foreign user to project and get note?


### Cookie Signing Key?
The cookie signing key was in the wwwroot, and thus served to any requesting user agent.

### Project UUID Leak?
The UUID of a project is leaked (incremental? through dirlisting? just because?), and can be used to plan capacity for any project, whose details are visible in the personal page

### Department UUID Leak?
The UUID of a department is leaked, an can be used to join a department

### Secondary Ideas
- leak something built into the frontend?
- leak something with debug exception pages?
    - exception, stacktrace?
- flags in filenames, dirlisting enabled? DirectoryBrowserOptions may differ from StaticFileOptions
    - leak UUIDs?
- leak signing key thorugh LFI?
- new Guid()?
- Change Id of your department somehow, retain old session?


# TODO
- index over departmentname
- unique index over TotalPlanning's (emplyeeId, projectId)
