using System;
using MySql.Data.MySqlClient;

class Program
{
    static string connectionString = "Server=localhost;Database=my_database;Uid=root;Pwd=Tza3bros123$;";

    static void Main(string[] args)
    {
        Console.WriteLine("Choose an operation: Add, Update, Delete");
        string operation = Console.ReadLine().ToLower();

        switch (operation)
        {
            case "add":
                AddStudent();
                break;
            case "update":
                UpdateStudent();
                break;
            case "delete":
                DeleteStudent();
                break;
            default:
                Console.WriteLine("Invalid operation.");
                break;
        }
    }

    static void AddStudent()
{
    Console.WriteLine("Enter Last Name:");
    string lastName = Console.ReadLine();

    Console.WriteLine("Enter First Name:");
    string firstName = Console.ReadLine();

    Console.WriteLine("Enter Net ID:");
    string netID = Console.ReadLine();

    Console.WriteLine("Enter UTD ID:");
    string utdID = Console.ReadLine();

    // Default role is 'Student'
    string userRole = "Student";

    using (MySqlConnection conn = new MySqlConnection(connectionString))
    {
        try
        {
            string query = "INSERT INTO users (utd_id, net_id, password, last_name, first_name, user_role) VALUES (@UtdId, @NetId, 'default123', @LastName, @FirstName, @UserRole)";
            MySqlCommand cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@UtdId", utdID);
            cmd.Parameters.AddWithValue("@NetId", netID);
            cmd.Parameters.AddWithValue("@LastName", lastName);
            cmd.Parameters.AddWithValue("@FirstName", firstName);
            cmd.Parameters.AddWithValue("@UserRole", userRole);

            conn.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            conn.Close();

            Console.WriteLine($"{rowsAffected} row(s) added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}

    static void UpdateStudent()
    {
        Console.WriteLine("Enter UTD ID of Student to Update:");
        string utdID = Console.ReadLine();

        Console.WriteLine("Enter New Last Name (Leave blank to skip):");
        string newLastName = Console.ReadLine();

        Console.WriteLine("Enter New First Name (Leave blank to skip):");
        string newFirstName = Console.ReadLine();

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            string query = "UPDATE users SET ";
            if (!string.IsNullOrEmpty(newLastName)) query += "last_name = @NewLastName, ";
            if (!string.IsNullOrEmpty(newFirstName)) query += "first_name = @NewFirstName, ";
            query = query.TrimEnd(',', ' ') + " WHERE utd_id = @UTDID";

            MySqlCommand cmd = new MySqlCommand(query, conn);

            if (!string.IsNullOrEmpty(newLastName)) cmd.Parameters.AddWithValue("@NewLastName", newLastName);
            if (!string.IsNullOrEmpty(newFirstName)) cmd.Parameters.AddWithValue("@NewFirstName", newFirstName);
            cmd.Parameters.AddWithValue("@UTDID", utdID);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            Console.WriteLine("Student updated successfully!");
        }
    }

    static void DeleteStudent()
    {
        Console.WriteLine("Enter UTD ID of Student to Delete:");
        string utdID = Console.ReadLine();

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            string query = "DELETE FROM users WHERE utd_id = @UTDID";
            MySqlCommand cmd = new MySqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@UTDID", utdID);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            Console.WriteLine("Student deleted successfully!");
        }
    }
}

