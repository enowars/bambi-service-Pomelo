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

## Flagstores
### User Field: Notes
### Planned Hours Field: Comment

### Fileuploads
The "SAP Export" files contain flags.

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

## Vulns

### Register with same username and create new project
The project creation leaks all properties of first user with that name, including the note field.

### Join forgein project and get details

### Add foreign user to project and get note?

### DB Leak?
The sqlite3 database was in the wwwroot, and thus served to any requesting user agent.

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
