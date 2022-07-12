# Hotel Cancún

 <p>
  This repository is the result of a programming test for an international job opportunity. The requirements, as well as possible improvements towards the project are listed below.
 </p>
  
  ## Overview
  
  <p>
  World is recovering from the recent pandemic, but unfortunately, a lot of hotels have been shut down. Many tourist places are left with only one hotel, and that is the case of Cancún.
  This project consists of an API capable of placing, consulting, changing and canceling reservations for this hotel.
  </p>
  
  ## Technical definitions
  
- Solution is a Restful Web API made in C# .net 6.0, following a Domain Driven Design (DDD) pattern;
- When it sets up, it loads and intance of EF Core in-memory database. It was chosen this way because it would be easier to load and test, since it doesn't need any additional files or installation. Also, it can be configured to connect to SQL Server (or any other database) with few code changes;
- There is an unit test project, covering the operations regarding the Reservations.

<h5>System requirements (as described in the challenge's instructions):</h5>

```
- API will be maintained by the hotel’s IT department.
- As it’s the very last hotel, the quality of service must be 99.99 to 100% => no downtime
- For the purpose of the test, we assume the hotel has only one room available
- To give a chance to everyone to book the room, the stay can’t be longer than 3 days and
can’t be reserved more than 30 days in advance.
- All reservations start at least the next day of booking,
- To simplify the use case, a “DAY’ in the hotel room starts from 00:00 to 23:59:59.
- Every end-user can check the room availability, place a reservation, cancel it or modify it.
- To simplify the API is insecure
```

## Setup and running

First, clone the repository using the git command or the most preferred way:

```
git clone https://github.com/TheLeao/HotelCancun.git
```

The SDK for .Net 6 must be installed in order to run the solution. It can be downloaded here: https://dotnet.microsoft.com/en-us/download
Then, it can be started by running the following command through a terminal, in the solution folder:

```
dotnet run
```

Or be initiated directly from Visual Studio 2022.

The API should be running in the addresses 'https://localhost:7209' and 'http://localhost:5209' and ready to listen to requests.

## Reservations

The reservations are the domain entity that is worked with in this project. All projects of the solution are aimed towards operating directly with it. The Reservation class has the following properties:
- StartDate (DateTime)
- EndDate (DateTime)
- Canceled (Bool)
- ReservedBy (String)

The dates represent the beginning and end of the stay in the hotel's room. 
The boolean property informs if the reservation has been canceled. After being canceled, the same reservation cannot be reopened. 
And lastly, a string indicating to whom the reservation is placed for.

The inclusion of the ReservedBy property in this scenario is to enforce the rule "**To give a chance to everyone to book the room** the stay can’t be longer than 3 days" of the requirements. By knowing who is booking the room, is possible to prevent the same person from reserving more than 3 days straight, through multiple reservations.
