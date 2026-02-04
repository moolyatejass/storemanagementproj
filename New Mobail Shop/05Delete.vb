Imports System.Data
Imports System.Data.SqlClient

Public Class Delete

    ' --- Centralized Connection String (Includes TrustServerCertificate=True) ---
    Private ReadOnly connectionString As String = "Data Source=LOQI\SQLEXPRESS;Initial Catalog=MobileShopManagementSystem;Integrated Security=True;TrustServerCertificate=True;"

    ' --- Event handler for Button1 click (Handles the "Soft Delete" - Mark as Inactive Action) ---
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim tableName As String = ""
        Dim recordId As Integer
        Dim recordType As String = "" ' Used for user-friendly messages

        ' --- 1. Validate User Input ---
        If ComboBox2.SelectedIndex = -1 Then
            MessageBox.Show("Please select a record type (Customer or Mobile) to mark as inactive.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ComboBox2.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(TextBox1.Text) Then
            MessageBox.Show("Please enter the ID of the record you want to mark as inactive.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox1.Focus()
            Return
        End If

        If Not Integer.TryParse(TextBox1.Text.Trim(), recordId) Then
            MessageBox.Show("Please enter a valid numeric ID.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox1.SelectAll()
            TextBox1.Focus()
            Return
        End If

        ' Determine table name and type based on ComboBox selection
        If ComboBox2.SelectedIndex = 0 Then
            tableName = "customer"
            recordType = "Customer"
        ElseIf ComboBox2.SelectedIndex = 1 Then
            tableName = "mobile"
            recordType = "Mobile"
        Else
            ' This case should ideally not be reachable if validation above works
            MessageBox.Show("Invalid record type selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' --- 2. Confirm Action with the User ---
        Dim confirmResult As DialogResult = MessageBox.Show($"Are you sure you want to mark {recordType} record with ID: {recordId} as inactive?" & vbCrLf & vbCrLf &
                                                          "It will be hidden from most views but can be recovered later if needed.",
                                                          "Confirm Inactivation", ' Updated title
                                                          MessageBoxButtons.YesNo,
                                                          MessageBoxIcon.Question, ' Updated icon
                                                          MessageBoxDefaultButton.Button2) ' Default to No

        If confirmResult = DialogResult.No Then
            Return ' User chose not to proceed
        End If

        ' --- 3. Perform the Database UPDATE (Set IsActive = 0) ---
        Dim success As Boolean = False
        ' SQL command to mark the record as inactive (IsActive = 0)
        ' Added "AND IsActive = 1" to only update records that are currently active
        Dim sql As String = $"UPDATE {tableName} SET IsActive = 0 WHERE id = @id AND IsActive = 1"

        ' Use Using blocks for automatic disposal of connection and command
        Using con As New SqlConnection(connectionString)
            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@id", recordId)

                Try
                    con.Open()
                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery() ' Execute the UPDATE statement

                    If rowsAffected > 0 Then
                        ' Update was successful
                        MessageBox.Show($"{recordType} record (ID: {recordId}) marked as inactive successfully.", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        success = True
                    Else
                        ' No rows affected - Could be wrong ID or already inactive
                        MessageBox.Show($"Could not mark {recordType} (ID: {recordId}) as inactive. It may not exist or was already inactive.", "Record Not Found or Already Inactive", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    End If

                Catch exSql As SqlException
                    ' Handle general SQL errors during UPDATE
                    MessageBox.Show($"Database Error updating record status: {exSql.Message}{vbCrLf}{vbCrLf}" &
                                    $"SQL State: {exSql.State}, Error Number: {exSql.Number}",
                                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

                Catch ex As Exception
                    ' Handle any other unexpected errors
                    MessageBox.Show($"An unexpected error occurred during update: {ex.Message}", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
                ' Connection automatically closed by Using block here
            End Using ' cmd is disposed here
        End Using ' con is disposed here

        ' --- 4. Refresh Grid and Clear Input if Update Succeeded ---
        If success Then
            ' Reload data for the currently selected table type to show the change (will exclude the inactive one)
            LoadData(tableName)

            ' Clear the ID input box
            TextBox1.Clear()
        End If

    End Sub

    ' --- Event handler for ComboBox2 selection change ---
    ' Loads the ACTIVE data for the selected table type into the DataGridView
    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        If ComboBox2.SelectedIndex = 0 Then
            LoadData("customer") ' Load active customer data
            TextBox1.Clear()    ' Clear ID box when table changes
            TextBox1.Focus()    ' Set focus to ID box
        ElseIf ComboBox2.SelectedIndex = 1 Then
            LoadData("mobile")   ' Load active mobile data
            TextBox1.Clear()    ' Clear ID box when table changes
            TextBox1.Focus()    ' Set focus to ID box
        Else ' Handle "--Select--" or other invalid index
            DataGridView1.DataSource = Nothing ' Clear the grid
            TextBox1.Clear()                 ' Clear the ID box
        End If
    End Sub

    ' --- Shared Helper method to load ACTIVE data into the DataGridView ---
    ' Takes the table name as input and populates DataGridView1 with IsActive = 1 records
    Private Sub LoadData(tableName As String)
        Dim localDt As New DataTable() ' Use a local DataTable for robust binding
        ' UPDATED SQL to filter for active records
        Dim sql As String = $"SELECT * FROM {tableName} WHERE IsActive = 1 ORDER BY id"

        ' Use Using blocks for automatic resource disposal
        Using con As New SqlConnection(connectionString)
            Using cmd As New SqlCommand(sql, con)
                Try
                    ' Use SqlDataAdapter to fill the local DataTable
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(localDt)

                    ' Bind the DataGridView to the newly filled local DataTable
                    DataGridView1.DataSource = Nothing ' Unbind previous source first
                    DataGridView1.DataSource = localDt ' Set the new data source

                    ' Optional: Adjust column widths automatically if data exists
                    If localDt.Rows.Count > 0 Then
                        DataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
                    Else
                        ' If the table is empty, adjust sizing appropriately
                        DataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.None) ' Reset sizing if empty
                        DataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells) ' Resize header/visible cells
                    End If

                Catch exSql As SqlException
                    MessageBox.Show($"SQL Error loading active data for '{tableName}': {exSql.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    DataGridView1.DataSource = Nothing ' Clear grid on error
                Catch ex As Exception
                    MessageBox.Show($"An unexpected error occurred while loading active data: {ex.Message}", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    DataGridView1.DataSource = Nothing ' Clear grid on error
                End Try
                ' Connection automatically closed by Using block here
            End Using ' cmd is disposed here
        End Using ' con is disposed here
    End Sub

    ' --- Event handler for form load ---
    ' Initializes the form controls when it first loads
    Private Sub Delete_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initialize ComboBox items
        ComboBox2.Items.Clear()       ' Clear any items added in the designer
        ComboBox2.Items.Add("Customer") ' Index 0
        ComboBox2.Items.Add("Mobile")   ' Index 1
        ComboBox2.DropDownStyle = ComboBoxStyle.DropDownList ' Prevent user typing invalid entries
        ComboBox2.SelectedIndex = -1  ' No initial selection
        ' ComboBox2.Text = "--Select--" ' Placeholder text (may not show if DropDownList style is used)

        ' Clear the grid and textbox initially
        DataGridView1.DataSource = Nothing
        TextBox1.Clear()

        ' Set initial focus maybe?
        ComboBox2.Focus()
    End Sub

End Class