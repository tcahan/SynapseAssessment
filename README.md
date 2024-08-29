# SynapseAssessment

This is the repository for the completion of the tech assessment for Synapse Health. I am provided with the following scenario.

> We have implemented a background process responsible for monitoring order status with the objective of identifying orders that have reached a 'delivered' state. Upon detection of such orders, a delivery notification is dispatched through a secondary API. After the notification is sent, we then update the order record via API.
>
> Regrettably, the developer responsible for delivering this essential functionality won the lottery and quit the next day. On their departure, they forwarded us a segment of code pertaining to this task. While we have reasonable confidence in its functionality, a preliminary review indicates deviations from established best practices. Adding to the complexity, the absence of the Product Owner (PO) due to vacation without access to cellular communication means that clarifications on the code or its requirements cannot be promptly obtained.
>
> This situation places us at a crossroads, as the business is eager to expedite the integration of this feature into the production environment. Thus, we're venturing forth to revisit and refine the provided code. Your job is to fix up the code, make sure it fits current coding best practices and make the code more supportable.

## Requirements

The following are requirments provided for the assessment.

- Solution includes at least 1 Unit test.
- Do not send back a single file answer.
  - At the very least 2 .net projects are expected (a unit test project plus whatever you need).
- Make sure this is “Production ready” or as close as you can get.
  - Make notes via comments in places where this is not possible.
- Please add logging where appropriate.
  - Console is not logging.
- Make sure your solution is .Net 6+
- Some way of mocking the API calls.
- Please send the .net solution zipped up via email or send a link to your code.
  - If you send a link, please make sure your repo/dropbox/google drive/etc is public.

## Instructions

Running the application will be fairly straightforward as this is going to be a common console application and test project. However, some configuration will be required, or at least recommended.

### Acquire the code

Clone or download a zip of the repository from GitHub.

_Additional instructions may be provided depending on time. These are common practices, so may not truly be necessary._

### SynapseAssessment Application

This is the main application that has taken the provided code and refactored into something production ready. As part of this process, a standard appsettings.json configuration file has been included in the project. Please configure the following settings as needed.

- ordersApiUrl
- alertApiUrl
- updateApiUrl
- Logging level

_Section will be updated with actual configuration names once implemented_

## Design Decisions

### Logging - Serilog

Logging will be handled with Serilog. This will still output to the console and debug, but can easily be configured to write to a database log table once one is included in the app.

### Testing - xUnit

As I have some experience with xUnit for testing, as well as it being a popular testing solution, I will be implenting the test requirement using it.

### Error Handling

I thought about a couple of different ways of handling the errors with the application. Originally I had considered an approach that would catch and throw the errors in each call of my ApiService class. This would allow me to provide a custom error message, specific to the issue, and then throw this ultimately to the *Start* method's catch block. I decided against this approach to allow for not always stopping the application on less severe issues. Additionally, since some calls are nested, the bubbling up of the exceptions with custom messages could start to get messy.

I decided on another common approach of just writing one try/catch block to handle exceptions around the main entry point in *Start*. This will simplify the exception handling. This is the approach I have often taken in other applications where the event handler has a try/catch and anything called by that handler does not have its own.

Additionally, I noted in the comments about stack trace and its inclusion in logging. For this implementation I chose not to include the stack trace as things will get very messy for the log messages. I offered a couple of alternative solutions for this to either log a lower level message for the stack trace, useful for debugging a known issue, or the option of creating an error table in a database that handles the detailed specifics of exceptions only, no other log messages. The table would provide a good way for someone running this application to point to a specific issue they had a problem with by referencing a returned error id value. Very helpful in a scenario where users can create support tickets as well as just for debugging problems.

### Method Naming

Renamed the *SendAlertAndUpdateOrder* method to just *UpdateOrder*. There is currently no logic to send an alert here as it is handled in the *ProcessOrder* method when status is delivered.

_Section will be updated as decisions are made._
