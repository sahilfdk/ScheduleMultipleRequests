# ScheduleMultipleRequests

ScheduleMultipleRequests is a .NET Core console application designed to send scheduled API requests automatically using multiple accounts in parallel. This application waits until a specified time (e.g., 2:00 PM) and then makes asynchronous API calls using different sets of cookies for different accounts.

## Features

- Schedules API requests to start at a specific system time.
- Makes parallel asynchronous API calls using multiple accounts.
- Supports custom cookies for different accounts.
- Configurable number of API call iterations.

## Prerequisites

- .NET Core SDK
- Visual Studio or any other C# development environment

## Installation

1. Clone the repository:

   ```sh
   git clone https://github.com/yourusername/ScheduleMultipleRequests.git
   cd ScheduleMultipleRequests
