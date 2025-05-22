# Student Management System v2.0

This is a console-based Student Management System built with VB.NET that provides a comprehensive set of functionalities for managing student records, including database operations, data import, and logging.

## Features

The application offers the following features:

1.  **View All Students:** Display a formatted list of all students currently in the database.
2.  **Add Student:** Add a new student record with a name and grade. Includes input validation.
3.  **Update Student:** Modify an existing student's name and grade based on their ID.
4.  **Delete Student:** Remove a student record from the database using their ID.
5.  **Search Students:** Find students by name (partial match) or exact ID.
6.  **Sort Students:** Sort the student list by name (ascending) or grade (descending).
7.  **View Statistics:** Display key statistics such as total students, average grade, highest grade, and lowest grade.
8.  **Import Students from CSV:** Import student data from a specified CSV file.
9.  **Backup Database:** Create a timestamped backup of the `SchoolDB.db` database.
10. **View Action Log:** Display the last 10 actions performed within the system.
11. **Exit:** Close the application.

## Technologies Used

* **Language:** VB.NET
* **Database:** SQLite (managed by `Microsoft.Data.Sqlite`)
* **CSV Handling:** `CsvHelper` library
* **Core Libraries:** `System`, `System.IO`, `System.Linq`, `System.Collections.Generic`, `System.Globalization`

## Prerequisites

To compile and run this application, you will need:

* .NET SDK (compatible with VB.NET)
* The `Microsoft.Data.Sqlite` NuGet package.
* The `CsvHelper` NuGet package.

You can install the NuGet packages using the .NET CLI:

```bash
dotnet add package Microsoft.Data.Sqlite
dotnet package add CsvHelper
```

## Setup

1.  **Clone the repository** (or save the provided code to a `.vb` file).
2.  **Create a new .NET Console Application project** if you haven't already.
3.  **Add the NuGet packages** as mentioned in the "Prerequisites" section.
4.  **Ensure a database file exists:** The application expects a SQLite database file named `SchoolDB.db` in the same directory as the executable. If it doesn't exist, the application will attempt to create it.
    * **Initial Database Setup:** You might need to manually create the `Students` and `ActionLog` tables if they don't exist in `SchoolDB.db` on the first run. A simple SQL script to do this would be:

        ```sql
        CREATE TABLE IF NOT EXISTS Students (
            ID INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Grade INTEGER NOT NULL,
            DateAdded DATETIME DEFAULT CURRENT_TIMESTAMP
        );

        CREATE TABLE IF NOT EXISTS ActionLog (
            ID INTEGER PRIMARY KEY AUTOINCREMENT,
            Action TEXT NOT NULL,
            Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
        );
        ```

        You can execute these commands using a SQLite browser or by adding initial setup logic to the `Main` subroutine if the database file is not found.

## Usage

1.  **Compile the application:**
    ```bash
    dotnet build
    ```
2.  **Run the application:**
    ```bash
    dotnet run
    ```
3.  Follow the on-screen menu prompts to interact with the Student Management System.

### CSV Import Format

For the "Import Students from CSV" feature (Option 8), the CSV file should have at least two columns: `Name` and `Grade`. For example:

```csv
Name,Grade
Alice,85
Bob,92
Charlie,78
```

## Database Schema

The application uses a SQLite database named `SchoolDB.db` with the following tables:

### `Students` Table

| Column    | Type        | Constraints                 | Description                               |
| :-------- | :---------- | :-------------------------- | :---------------------------------------- |
| `ID`      | `INTEGER`   | `PRIMARY KEY AUTOINCREMENT` | Unique identifier for each student.       |
| `Name`    | `TEXT`      | `NOT NULL`                  | The student's name (max 100 characters).  |
| `Grade`   | `INTEGER`   | `NOT NULL`                  | The student's grade (0-100).              |
| `DateAdded` | `DATETIME` | `DEFAULT CURRENT_TIMESTAMP` | Timestamp when the student record was added. |

### `ActionLog` Table

| Column      | Type        | Constraints                 | Description                               |
| :---------- | :---------- | :-------------------------- | :---------------------------------------- |
| `ID`        | `INTEGER`   | `PRIMARY KEY AUTOINCREMENT` | Unique identifier for each log entry.     |
| `Action`    | `TEXT`      | `NOT NULL`                  | Description of the action performed.      |
| `Timestamp` | `DATETIME` | `DEFAULT CURRENT_TIMESTAMP` | Timestamp when the action was logged.     |
