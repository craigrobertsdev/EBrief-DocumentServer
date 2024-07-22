# EBrief Document Server

## Table of Contents

- [Description](#description)
- [Installation](#installation)
- [Usage](#usage)
- [Questions](#questions)
- [Licence](#licence)

## Description

This application provides dummy data to the [EBrief](https://github.com/craigrobertsdev/EBrief-Demo) application.

## Installation

Clone the repo to your local machine:

```bash
  git clone https://github.com/craigrobertsdev/DocumentServer.git
```

<br>

You will need the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed.

## Usage

1. Start the document server:
   In your terminal, navigate to the folder containing the Document Server and run the following command:

```bash
dotnet run -lp https
```

  <br>

2. Start the EBrief application:
   Run the application from your text editor of choice, or from the terminal using the following command:

```bash
dotnet run
```

<br>

3. The first time you launch the application, you will need to generate a court list by clicking on the `Load New Court List` button. Fill in the information in the dropdowns (it is all test data so you can choose any option) and provide a list of unique numbers separated by spaces or line breaks. A sample list is stored [here](https://github.com/craigrobertsdev/EBrief-Demo/blob/main/sample-case-numbers.txt).

## Questions

If you would like to discuss this project further, please contact me at craig.roberts11@outlook.com, or via [LinkedIn](https://www.linkedin.com/in/craig-roberts-9ba409243/).

## Licence

This application is subject to the MIT licence.
