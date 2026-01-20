Users have user accounts

Users have addresses

Users can have roles

Roles can be Admin | User | Driver

Users can Add Addresses, and request a Bin(s) to be delivered 

Payment is handled completely externally

Admins are in charge of managing users and their payments externally

Addresses with Bins get requested for approval by admins, for an admin to approve, it means the user paid for the bin externally

Addresses/Users can create ScheduleContracts - which has a driver collect rubish from the bin on a frequency, on a set day of the week (so every week/2 weeks/3 weeks/monthly)

SchedulleContracts get approved by admins based on external communication and payment.

System calculates schedules for the coming day for all the drivers based on their starting location

System generates a planned list of addresses closest to the driver for addresess and collection and drop off of garbage.

Drivers see this schedule every morning they log on.

Users can see notes and once garbage has been collected for the frequency as well as next collection.

No tracking, No reports, nothing special for admins, they simply manage user accounts, and can create addresses / Schedule Contracts on behalf of users AND approve user requests based on external payments.

Simple app - Use an External API for route planning and Auto Complete

------------------

When a customer is set as a driver in the front end - it should show additional detail in that update field requiring you to fill in their details, two endpoints will be sent.

We also need a depo table to manage those and we need to be able to manage them in the admin panel.

The hangfire job will manually trigger at 2am in the mornings but admin should be able to manually requeue work with a warning that existing work might get overwritten. (mostly for testing)

Users need to have a screen where they can view and manage their own details, this will be basic to start with. Just address creation with schedule creation.

These things will be in states, that the admin can manage.

Most of the communication for now happens outside the app, and drivers simply get routes to drive to.

------------------



