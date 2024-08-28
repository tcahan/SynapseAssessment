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

_Section will be updated as decisions are made._
