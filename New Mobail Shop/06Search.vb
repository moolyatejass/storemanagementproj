Imports System.Data
Imports System.Data.SqlClient

Public Class Search

    ' --- Centralized Connection String ---
    Private ReadOnly connectionString As String = "Data Source=LOQI\SQLEXPRESS;Initial Catalog=MobileShopManagementSystem;Integrated Security=True;TrustServerCertificate=True;"

    ' --- REMOVED class-level DataTable ---
    ' Using a local DataTable in ExecuteQuery is generally more robust for binding.
    ' Private dt As New DataTable()

    ' --- Event handler for Button1 click (Search by ID) ---
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim sql As String = ""
        Dim tableName As String = ""
        Dim searchId As Integer

        ' Determine table and SQL based on ComboBox selection
        If ComboBox2.SelectedIndex = 0 Then
            tableName = "customer"
            sql = $"SELECT * FROM {tableName} WHERE id = @id"
        ElseIf ComboBox2.SelectedIndex = 1 Then
            tableName = "mobile"
            sql = $"SELECT * FROM {tableName} WHERE id = @id"
        Else
            MessageBox.Show("Please select a table (Customer or Mobile) to search in.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return ' Exit if nothing is selected
        End If

        ' Validate TextBox input - Ensure it's a number
        If String.IsNullOrWhiteSpace(TextBox1.Text) Then
            MessageBox.Show("Please enter an ID to search for.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox1.Focus()
            Return
        End If

        If Not Integer.TryParse(TextBox1.Text.Trim(), searchId) Then
            MessageBox.Show("Please enter a valid numeric ID.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox1.Focus()
            Return
        End If

        ' --- Use a shared method for executing the query with the ID parameter ---
        ExecuteQuery(sql, New SqlParameter("@id", searchId))

    End Sub

    ' --- Event handler for ComboBox2 selection change (Load All Data) ---
    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        If ComboBox2.SelectedIndex = 0 Then
            LoadAllData("customer") ' Pass table name
        ElseIf ComboBox2.SelectedIndex = 1 Then
            LoadAllData("mobile") ' Pass table name
            ' Optional: Handle the case where the user selects "--Select--" again
            ' ElseIf ComboBox2.SelectedIndex = -1 Then
            '     DataGridView1.DataSource = Nothing ' Clear the grid
        End If
    End Sub

    ' --- Refactored method to load all data from a specified table ---
    Private Sub LoadAllData(tableName As String)
        Dim sql As String = $"SELECT * FROM {tableName}"
        ExecuteQuery(sql) ' Call the shared execution method without parameters
    End Sub

    ' --- Shared method to execute SQL query and populate DataGridView ---
    ' --- UPDATED: Uses a local DataTable for more robust binding ---
    Private Sub ExecuteQuery(sql As String, Optional parameter As SqlParameter = Nothing)

        ' *** Create a new DataTable instance for each query ***
        Dim localDt As New DataTable()

        ' Use Using blocks for automatic disposal of connection and command
        Using con As New SqlConnection(connectionString)
            Using com As New SqlCommand(sql, con)

                ' Add parameter if provided (for searching by ID)
                If parameter IsNot Nothing Then
                    com.Parameters.Add(parameter)
                End If

                Try
                    ' Use SqlDataAdapter to fill the local DataTable
                    Dim da As New SqlDataAdapter(com)
                    da.Fill(localDt)

                    ' *** Bind the DataGridView to the newly filled local DataTable ***
                    ' Unbind first (good practice)
                    DataGridView1.DataSource = Nothing
                    DataGridView1.DataSource = localDt ' Set the new data source

                    ' Optional: Adjust column widths automatically
                    If localDt.Rows.Count > 0 Then ' Avoid resizing if empty
                        DataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
                    ElseIf parameter IsNot Nothing Then ' Only show "not found" if specifically searching
                        MessageBox.Show("No record found with the specified ID.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If


                Catch exSql As SqlException
                    ' Provide more specific error handling for SQL issues
                    MessageBox.Show($"SQL Error executing query: {exSql.Message}{vbCrLf}{vbCrLf}Query: {sql}",
                                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    DataGridView1.DataSource = Nothing ' Clear grid on error
                Catch ex As Exception
                    ' Handle other potential errors during data fetching/binding
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}",
                                    "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    DataGridView1.DataSource = Nothing ' Clear grid on error
                End Try
                ' No Finally needed for con.Close() because Using block handles it automatically
            End Using ' com is disposed here
        End Using ' con is disposed here
    End Sub


    ' --- Event handler for form load ---
    Private Sub Search_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initialize ComboBox items
        ComboBox2.Items.Clear() ' Clear any design-time items first
        ComboBox2.Items.Add("Customer") ' Index 0
        ComboBox2.Items.Add("Mobile")   ' Index 1
        ComboBox2.SelectedIndex = -1    ' Ensure nothing is selected initially
        ComboBox2.Text = "--Select--"  ' Set placeholder text

        ' Optional: Clear the grid initially
        DataGridView1.DataSource = Nothing
        TextBox1.Text = "" ' Clear search box initially
    End Sub

End Class