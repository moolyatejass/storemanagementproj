Imports System.Data.SqlClient

Public Class Form1

    Private Const UsernamePlaceholder As String = "Username"
    Private Const PasswordPlaceholder As String = "Password"

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        username.Text = UsernamePlaceholder
        password.Text = PasswordPlaceholder

        password.PasswordChar = ControlChars.NullChar
    End Sub

    Private Sub txtUsername_Enter(sender As Object, e As EventArgs) Handles username.Enter
        If username.Text = UsernamePlaceholder Then
            username.Text = ""
        End If
    End Sub

    Private Sub txtUsername_Leave(sender As Object, e As EventArgs) Handles username.Leave
        If String.IsNullOrWhiteSpace(username.Text) Then
            username.Text = UsernamePlaceholder
        End If
    End Sub

    Private Sub txtPassword_Enter(sender As Object, e As EventArgs) Handles password.Enter
        If password.Text = PasswordPlaceholder Then
            password.Text = ""
            password.PasswordChar = "*" ' Enable password masking.
        End If
    End Sub

    Private Sub txtPassword_Leave(sender As Object, e As EventArgs) Handles password.Leave
        If String.IsNullOrWhiteSpace(password.Text) Then
            password.Text = PasswordPlaceholder
            password.PasswordChar = ControlChars.NullChar ' Remove masking to show placeholder.
        End If
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim enteredUsername As String = username.Text.Trim()
        Dim enteredPassword As String = password.Text

        ' Check if placeholders are still present
        If enteredUsername = UsernamePlaceholder OrElse enteredPassword = PasswordPlaceholder Then
            MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        ' Validate credentials against the database
        If ValidateUser(enteredUsername, enteredPassword) Then
            MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Dim homepageForm As New Home
            homepageForm.Show()
            Me.Hide()
        Else
            MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If


    End Sub

    Private Function ValidateUser(username As String, password As String) As Boolean
        ' Connection string to your SQL Server database
        Dim connectionString As String = "Data Source=LOQI\SQLEXPRESS;Initial Catalog=MobileShopManagementSystem;Integrated Security=True;TrustServerCertificate=True;"

        ' Query to check if the user exists
        Dim query As String = "SELECT COUNT(*) FROM login WHERE Username = @Username AND Password = @Password"

        Using connection As New SqlConnection(connectionString)
            Using command As New SqlCommand(query, connection)
                ' Add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@Username", username)
                command.Parameters.AddWithValue("@Password", password)

                Try
                    connection.Open()
                    Dim count As Integer = Convert.ToInt32(command.ExecuteScalar())
                    Return count > 0
                Catch ex As Exception
                    MessageBox.Show("Error connecting to the database: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False
                End Try
            End Using
        End Using
    End Function


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim resetForm As New Form2
        resetForm.Show()
        Me.Hide()
    End Sub
End Class

