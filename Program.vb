Imports System
Imports Microsoft.Data.Sqlite
Imports CsvHelper
Imports System.IO
Imports System.Linq
Imports System.Collections.Generic
Imports System.Globalization

Module Program
    Sub Main(args As String())
        Dim connectionString As String = "Data Source=SchoolDB.db"
        Console.Title = "Student Management System"

        While True
            ClearScreen()
            DisplayHeader()
            DisplayMenu()
            Console.Write("Select an option (1-10): ")
            Dim choice As String = Console.ReadLine()

            Select Case choice
                Case "1" : ViewStudents(connectionString)
                Case "2" : AddStudent(connectionString)
                Case "3" : UpdateStudent(connectionString)
                Case "4" : DeleteStudent(connectionString)
                Case "5" : SearchStudents(connectionString)
                Case "6" : SortStudents(connectionString)
                Case "7" : ViewStatistics(connectionString)
                Case "8" : ImportStudentsFromCsv(connectionString)
                Case "9" : BackupDatabase()
                Case "10" : ViewActionLog(connectionString)
                Case "11" : Console.WriteLine(vbCrLf & "Goodbye!"): Exit While
                Case Else : DisplayError("Invalid option. Try again.")
            End Select
            Console.Write(vbCrLf & "Press any key to continue...")
            Console.ReadKey()
        End While
    End Sub

    Sub ClearScreen()
        Console.Clear()
    End Sub

    Sub DisplayHeader()
        Console.ForegroundColor = ConsoleColor.Cyan
        Console.WriteLine("======================================")
        Console.WriteLine("    Student Management System v2.0    ")
        Console.WriteLine("======================================")
        Console.ResetColor()
    End Sub

    Sub DisplayMenu()
        Console.WriteLine(vbCrLf & "Menu Options:")
        Console.WriteLine("1. View All Students")
        Console.WriteLine("2. Add Student")
        Console.WriteLine("3. Update Student")
        Console.WriteLine("4. Delete Student")
        Console.WriteLine("5. Search Students")
        Console.WriteLine("6. Sort Students")
        Console.WriteLine("7. View Statistics")
        Console.WriteLine("8. Import Students from CSV")
        Console.WriteLine("9. Backup Database")
        Console.WriteLine("10. View Action Log")
        Console.WriteLine("11. Exit")
    End Sub

    Sub DisplaySuccess(message As String)
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine(message)
        Console.ResetColor()
    End Sub

    Sub DisplayError(message As String)
        Console.ForegroundColor = ConsoleColor.Red
        Console.WriteLine(message)
        Console.ResetColor()
    End Sub

    Sub LogAction(connectionString As String, action As String)
        Try
            Using connection As New SqliteConnection(connectionString)
                connection.Open()
                Dim query As String = "INSERT INTO ActionLog (Action) VALUES (@Action)"
                Using command As New SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@Action", action)
                    command.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            DisplayError("Failed to log action: " & ex.Message)
        End Try
    End Sub

    Sub ViewStudents(connectionString As String)
        Try
            Using connection As New SqliteConnection(connectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM Students"
                Using command As New SqliteCommand(query, connection)
                    Using reader As SqliteDataReader = command.ExecuteReader()
                        ClearScreen()
                        DisplayHeader()
                        Console.WriteLine(vbCrLf & "Students List:")
                        Console.WriteLine("-------------------------------------------------------")
                        Console.WriteLine("| {0,-5} | {1,-20} | {2,-6} | {3,-19} |", "ID", "Name", "Grade", "Date Added")
                        Console.WriteLine("-------------------------------------------------------")
                        If Not reader.HasRows Then
                            Console.WriteLine("| No students found.                                 |")
                        Else
                            While reader.Read()
                                Console.WriteLine("| {0,-5} | {1,-20} | {2,-6} | {3,-19} |", reader("ID"), reader("Name").ToString().Substring(0, Math.Min(20, reader("Name").ToString().Length)), reader("Grade"), reader("DateAdded"))
                            End While
                        End If
                        Console.WriteLine("-------------------------------------------------------")
                    End Using
                End Using
            End Using
            LogAction(connectionString, "Viewed all students")
        Catch ex As Exception
            DisplayError("Error: " & ex.Message)
        End Try
    End Sub

    Sub AddStudent(connectionString As String)
        Try
            ClearScreen()
            DisplayHeader()
            Console.WriteLine(vbCrLf & "Add New Student:")
            Console.Write("Enter student name: ")
            Dim name As String = Console.ReadLine()
            If String.IsNullOrWhiteSpace(name) OrElse name.Length > 100 Then
                DisplayError("Name cannot be empty or exceed 100 characters.")
                Return
            End If

            Console.Write("Enter student grade (0-100): ")
            Dim gradeInput As String = Console.ReadLine()
            Dim grade As Integer
            If Not Integer.TryParse(gradeInput, grade) OrElse grade < 0 OrElse grade > 100 Then
                DisplayError("Invalid grade. Must be between 0 and 100.")
                Return
            End If

            Using connection As New SqliteConnection(connectionString)
                connection.Open()
                Dim query As String = "INSERT INTO Students (Name, Grade) VALUES (@Name, @Grade)"
                Using command As New SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@Name", name)
                    command.Parameters.AddWithValue("@Grade", grade)
                    command.ExecuteNonQuery()
                    DisplaySuccess("Student added successfully!")
                    LogAction(connectionString, $"Added student: {name}")
                End Using
            End Using
        Catch ex As Exception
            DisplayError("Error: " & ex.Message)
        End Try
    End Sub

    Sub UpdateStudent(connectionString As String)
        Try
            ClearScreen()
            DisplayHeader()
            Console.WriteLine(vbCrLf & "Update Student:")
            Console.Write("Enter student ID to update: ")
            Dim idInput As String = Console.ReadLine()
            Dim id As Integer
            If Not Integer.TryParse(idInput, id) Then
                DisplayError("Invalid ID.")
                Return
            End If

            Console.Write("Enter new name: ")
            Dim name As String = Console.ReadLine()
            If String.IsNullOrWhiteSpace(name) OrElse name.Length > 100 Then
                DisplayError("Name cannot be empty or exceed 100 characters.")
                Return
            End If

            Console.Write("Enter new grade (0-100): ")
            Dim gradeInput As String = Console.ReadLine()
            Dim grade As Integer
            If Not Integer.TryParse(gradeInput, grade) OrElse grade < 0 OrElse grade > 100 Then
                DisplayError("Invalid grade. Must be between 0 and 100.")
                Return
            End If

            Using connection As New SqliteConnection(connectionString)
                connection.Open()
                Dim query As String = "UPDATE Students SET Name = @Name, Grade = @Grade WHERE ID = @ID"
                Using command As New SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@Name", name)
                    command.Parameters.AddWithValue("@Grade", grade)
                    command.Parameters.AddWithValue("@ID", id)
                    Dim rowsAffected As Integer = command.ExecuteNonQuery()
                    If rowsAffected > 0 Then
                        DisplaySuccess("Student updated successfully!")
                        LogAction(connectionString, $"Updated student ID: {id}")
                    Else
                        DisplayError("Student ID not found.")
                    End If
                End Using
            End Using
        Catch ex As Exception
            DisplayError("Error: " & ex.Message)
        End Try
    End Sub

    Sub DeleteStudent(connectionString As String)
        Try
            ClearScreen()
            DisplayHeader()
            Console.WriteLine(vbCrLf & "Delete Student:")
            Console.Write("Enter student ID to delete: ")
            Dim idInput As String = Console.ReadLine()
            Dim id As Integer
            If Not Integer.TryParse(idInput, id) Then
                DisplayError("Invalid ID.")
                Return
            End If

            Using connection As New SqliteConnection(connectionString)
                connection.Open()
                Dim query As String = "DELETE FROM Students WHERE ID = @ID"
                Using command As New SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@ID", id)
                    Dim rowsAffected As Integer = command.ExecuteNonQuery()
                    If rowsAffected > 0 Then
                        DisplaySuccess("Student deleted successfully!")
                        LogAction(connectionString, $"Deleted student ID: {id}")
                    Else
                        DisplayError("Student ID not found.")
                    End If
                End Using
            End Using
        Catch ex As Exception
            DisplayError("Error: " & ex.Message)
        End Try
    End Sub

    Sub SearchStudents(connectionString As String)
        Try
            ClearScreen()
            DisplayHeader()
            Console.WriteLine(vbCrLf & "Search Students:")
            Console.Write("Enter name or ID to search (leave blank to cancel): ")
            Dim searchTerm As String = Console.ReadLine()
            If String.IsNullOrWhiteSpace(searchTerm) Then
                DisplayError("Search cancelled.")
                Return
            End If

            Using connection As New SqliteConnection(connectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM Students WHERE Name LIKE @SearchTerm OR ID = @ID"
                Using command As New SqliteCommand(query, connection)
                    Dim id As Integer
                    If Integer.TryParse(searchTerm, id) Then
                        command.Parameters.AddWithValue("@ID", id)
                    Else
                        command.Parameters.AddWithValue("@ID", 0)
                    End If
                    command.Parameters.AddWithValue("@SearchTerm", "%" & searchTerm & "%")
                    Using reader As SqliteDataReader = command.ExecuteReader()
                        Console.WriteLine(vbCrLf & "Search Results:")
                        Console.WriteLine("-------------------------------------------------------")
                        Console.WriteLine("| {0,-5} | {1,-20} | {2,-6} | {3,-19} |", "ID", "Name", "Grade", "Date Added")
                        Console.WriteLine("-------------------------------------------------------")
                        If Not reader.HasRows Then
                            Console.WriteLine("| No students found.                                 |")
                        Else
                            While reader.Read()
                                Console.WriteLine("| {0,-5} | {1,-20} | {2,-6} | {3,-19} |", reader("ID"), reader("Name").ToString().Substring(0, Math.Min(20, reader("Name").ToString().Length)), reader("Grade"), reader("DateAdded"))
                            End While
                        End If
                        Console.WriteLine("-------------------------------------------------------")
                    End Using
                End Using
            End Using
            LogAction(connectionString, $"Searched for: {searchTerm}")
        Catch ex As Exception
            DisplayError("Error: " & ex.Message)
        End Try
    End Sub

    Sub SortStudents(connectionString As String)
        Try
            ClearScreen()
            DisplayHeader()
            Console.WriteLine(vbCrLf & "Sort Students:")
            Console.WriteLine("1. Sort by Name (A-Z)")
            Console.WriteLine("2. Sort by Grade (High to Low)")
            Console.Write("Select sort option (1-2): ")
            Dim sortChoice As String = Console.ReadLine()
            Dim query As String

            Select Case sortChoice
                Case "1"
                    query = "SELECT * FROM Students ORDER BY Name ASC"
                Case "2"
                    query = "SELECT * FROM Students ORDER BY Grade DESC"
                Case Else
                    DisplayError("Invalid sort option.")
                    Return
            End Select

            Using connection As New SqliteConnection(connectionString)
                connection.Open()
                Using command As New SqliteCommand(query, connection)
                    Using reader As SqliteDataReader = command.ExecuteReader()
                        Console.WriteLine(vbCrLf & "Sorted Students:")
                        Console.WriteLine("-------------------------------------------------------")
                        Console.WriteLine("| {0,-5} | {1,-20} | {2,-6} | {3,-19} |", "ID", "Name", "Grade", "Date Added")
                        Console.WriteLine("-------------------------------------------------------")
                        If Not reader.HasRows Then
                            Console.WriteLine("| No students found.                                 |")
                        Else
                            While reader.Read()
                                Console.WriteLine("| {0,-5} | {1,-20} | {2,-6} | {3,-19} |", reader("ID"), reader("Name").ToString().Substring(0, Math.Min(20, reader("Name").ToString().Length)), reader("Grade"), reader("DateAdded"))
                            End While
                        End If
                        Console.WriteLine("-------------------------------------------------------")
                    End Using
                End Using
            End Using
            LogAction(connectionString, $"Sorted students by {If(sortChoice = "1", "name", "grade")}")
        Catch ex As Exception
            DisplayError("Error: " & ex.Message)
        End Try
    End Sub

    Sub ViewStatistics(connectionString As String)
        Try
            ClearScreen()
            DisplayHeader()
            Console.WriteLine(vbCrLf & "Student Statistics:")
            Using connection As New SqliteConnection(connectionString)
                connection.Open()
                Dim countQuery As String = "SELECT COUNT(*) FROM Students"
                Dim avgQuery As String = "SELECT AVG(Grade) FROM Students"
                Dim maxQuery As String = "SELECT MAX(Grade) FROM Students"
                Dim minQuery As String = "SELECT MIN(Grade) FROM Students"

                Dim totalStudents As Integer
                Dim avgGrade As Double
                Dim maxGrade As Integer
                Dim minGrade As Integer

                Using command As New SqliteCommand(countQuery, connection)
                    totalStudents = Convert.ToInt32(command.ExecuteScalar())
                End Using
                Using command As New SqliteCommand(avgQuery, connection)
                    avgGrade = If(command.ExecuteScalar() IsNot DBNull.Value, Convert.ToDouble(command.ExecuteScalar()), 0)
                End Using
                Using command As New SqliteCommand(maxQuery, connection)
                    maxGrade = If(command.ExecuteScalar() IsNot DBNull.Value, Convert.ToInt32(command.ExecuteScalar()), 0)
                End Using
                Using command As New SqliteCommand(minQuery, connection)
                    minGrade = If(command.ExecuteScalar() IsNot DBNull.Value, Convert.ToInt32(command.ExecuteScalar()), 0)
                End Using

                Console.WriteLine("-------------------------------------------------------")
                Console.WriteLine($"Total Students: {totalStudents}")
                Console.WriteLine($"Average Grade: {avgGrade:F2}")
                Console.WriteLine($"Highest Grade: {maxGrade}")
                Console.WriteLine($"Lowest Grade: {minGrade}")
                Console.WriteLine("-------------------------------------------------------")
            End Using
            LogAction(connectionString, "Viewed statistics")
        Catch ex As Exception
            DisplayError("Error: " & ex.Message)
        End Try
    End Sub

    Sub ImportStudentsFromCsv(connectionString As String)
        Try
            ClearScreen()
            DisplayHeader()
            Console.WriteLine(vbCrLf & "Import Students from CSV:")
            Console.Write("Enter CSV file path (e.g., students.csv): ")
            Dim csvPath As String = Console.ReadLine()
            If Not File.Exists(csvPath) Then
                DisplayError("File not found.")
                Return
            End If

            Dim imported As Integer = 0
            Using connection As New SqliteConnection(connectionString)
                connection.Open()
                Using reader As New StreamReader(csvPath)
                    Using csv As New CsvReader(reader, CultureInfo.InvariantCulture)
                        Dim records = csv.GetRecords(Of StudentCsv)().ToList()
                        For Each record In records
                            If String.IsNullOrWhiteSpace(record.Name) OrElse record.Name.Length > 100 OrElse record.Grade < 0 OrElse record.Grade > 100 Then
                                Continue For
                            End If
                            Dim query As String = "INSERT INTO Students (Name, Grade) VALUES (@Name, @Grade)"
                            Using command As New SqliteCommand(query, connection)
                                command.Parameters.AddWithValue("@Name", record.Name)
                                command.Parameters.AddWithValue("@Grade", record.Grade)
                                command.ExecuteNonQuery()
                                imported += 1
                            End Using
                        Next
                    End Using
                End Using
            End Using
            DisplaySuccess($"Imported {imported} students successfully!")
            LogAction(connectionString, $"Imported {imported} students from CSV")
        Catch ex As Exception
            DisplayError("Error: " & ex.Message)
        End Try
    End Sub

    Class StudentCsv
        Public Property Name As String
        Public Property Grade As Integer
    End Class

    Sub BackupDatabase()
        Try
            ClearScreen()
            DisplayHeader()
            Console.WriteLine(vbCrLf & "Backup Database:")
            Dim backupFile As String = $"SchoolDB_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db"
            File.Copy("SchoolDB.db", backupFile)
            DisplaySuccess($"Database backed up to {backupFile}!")
            LogAction("Data Source=SchoolDB.db", $"Created backup: {backupFile}")
        Catch ex As Exception
            DisplayError("Error: " & ex.Message)
        End Try
    End Sub

    Sub ViewActionLog(connectionString As String)
        Try
            ClearScreen()
            DisplayHeader()
            Console.WriteLine(vbCrLf & "Action Log:")
            Using connection As New SqliteConnection(connectionString)
                connection.Open()
                Dim query As String = "SELECT * FROM ActionLog ORDER BY Timestamp DESC LIMIT 10"
                Using command As New SqliteCommand(query, connection)
                    Using reader As SqliteDataReader = command.ExecuteReader()
                        Console.WriteLine("-------------------------------------------------------")
                        Console.WriteLine("| {0,-5} | {1,-30} | {2,-19} |", "ID", "Action", "Timestamp")
                        Console.WriteLine("-------------------------------------------------------")
                        If Not reader.HasRows Then
                            Console.WriteLine("| No actions logged.                                |")
                        Else
                            While reader.Read()
                                Console.WriteLine("| {0,-5} | {1,-30} | {2,-19} |", reader("ID"), reader("Action").ToString().Substring(0, Math.Min(30, reader("Action").ToString().Length)), reader("Timestamp"))
                            End While
                        End If
                        Console.WriteLine("-------------------------------------------------------")
                    End Using
                End Using
            End Using
            LogAction(connectionString, "Viewed action log")
        Catch ex As Exception
            DisplayError("Error: " & ex.Message)
        End Try
    End Sub
End Module