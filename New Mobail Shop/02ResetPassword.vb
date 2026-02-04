Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Drawing ' Added for Color

Public Class Form2
    ' Updated connection string (FIXED)
    Private ReadOnly connectionString As String = "Data Source=LOQI\SQLEXPRESS;Initial Catalog=MobileShopManagementSystem;Integrated Security=True;TrustServerCertificate=True"

    ' Placeholder constants
    Private Const UserIDPlaceholder As String = "Enter User ID"
    Private Const SecurityAnswerPlaceholder As String = "Answer Security Question"
    Private Const NewPasswordPlaceholder As String = "New Password"
    Private Const ConfirmPasswordPlaceholder As String = "Confirm Password"

    ' --- Assume these controls exist on your Form2 designer ---
    ' Friend WithEvents txtUsername As TextBox
    ' Friend WithEvents txtSecurityAnswer As TextBox
    ' Friend WithEvents txtNewPassword As TextBox
    ' Friend WithEvents txtConfirmPassword As TextBox
    ' Friend WithEvents btnResetPassword As Button
    ' Friend WithEvents btnCancel As Button
    ' ----------------------------------------------------------

    ' Load event to initialize placeholders
    Private Sub ResetPasswordForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializePlaceholders()
    End Sub

    Private Sub InitializePlaceholders()
        SetPlaceholder(txtUsername, UserIDPlaceholder)
        SetPlaceholder(txtSecurityAnswer, SecurityAnswerPlaceholder)
        SetPlaceholder(txtNewPassword, NewPasswordPlaceholder, True)
        SetPlaceholder(txtConfirmPassword, ConfirmPasswordPlaceholder, True)
    End Sub

    ' --- Placeholder Helper Methods ---
    Private Sub SetPlaceholder(txtBox As TextBox, placeholder As String, Optional isPassword As Boolean = False)
        txtBox.Text = placeholder
        txtBox.ForeColor = Color.Gray
        If isPassword Then
            txtBox.PasswordChar = ControlChars.NullChar ' Show placeholder text initially
        End If
        ' Attach standard Enter/Leave handlers
        AddHandler txtBox.Enter, AddressOf TextBox_Enter
        AddHandler txtBox.Leave, AddressOf TextBox_Leave
    End Sub

    Private Sub RemovePlaceholder(txtBox As TextBox, Optional isPassword As Boolean = False)
        txtBox.Text = ""
        txtBox.ForeColor = Color.Black
        If isPassword Then
            txtBox.PasswordChar = "*" ' Use masking character
        End If
    End Sub

    ' Generic Enter event handler for textboxes with placeholders
    Private Sub TextBox_Enter(sender As Object, e As EventArgs)
        Dim txtBox As TextBox = CType(sender, TextBox)
        Dim isPassword As Boolean = (txtBox Is txtNewPassword OrElse txtBox Is txtConfirmPassword)
        Dim placeholder As String = GetPlaceholderText(txtBox)

        If txtBox.Text = placeholder Then
            RemovePlaceholder(txtBox, isPassword)
        End If
    End Sub

    ' Generic Leave event handler for textboxes with placeholders
    Private Sub TextBox_Leave(sender As Object, e As EventArgs)
        Dim txtBox As TextBox = CType(sender, TextBox)
        Dim isPassword As Boolean = (txtBox Is txtNewPassword OrElse txtBox Is txtConfirmPassword)
        Dim placeholder As String = GetPlaceholderText(txtBox)

        If String.IsNullOrWhiteSpace(txtBox.Text) Then
            SetPlaceholder(txtBox, placeholder, isPassword)
        ElseIf isPassword And txtBox.PasswordChar = ControlChars.NullChar Then
            ' Ensure password masking is reapplied if user types something then leaves
            txtBox.PasswordChar = "*"
        End If
    End Sub

    ' Helper to get the correct placeholder for a textbox
    Private Function GetPlaceholderText(txtBox As TextBox) As String
        If txtBox Is txtUsername Then Return UserIDPlaceholder
        If txtBox Is txtSecurityAnswer Then Return SecurityAnswerPlaceholder
        If txtBox Is txtNewPassword Then Return NewPasswordPlaceholder
        If txtBox Is txtConfirmPassword Then Return ConfirmPasswordPlaceholder
        Return "" ' Should not happen
    End Function
    ' ----------------------------------------

    ' Reset Password button event
    Private Sub btnResetPassword_Click(sender As Object, e As EventArgs) Handles btnResetPassword.Click
        Dim userID As String = If(txtUsername.Text = UserIDPlaceholder, "", txtUsername.Text.Trim())
        Dim securityAnswer As String = If(txtSecurityAnswer.Text = SecurityAnswerPlaceholder, "", txtSecurityAnswer.Text.Trim())
        Dim newPassword As String = If(txtNewPassword.Text = NewPasswordPlaceholder, "", txtNewPassword.Text) ' Don't trim passwords
        Dim confirmPassword As String = If(txtConfirmPassword.Text = ConfirmPasswordPlaceholder, "", txtConfirmPassword.Text) ' Don't trim passwords

        ' Validate input fields
        If String.IsNullOrWhiteSpace(userID) Then
            MessageBox.Show("Please enter your User ID.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtUsername.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(securityAnswer) Then
            MessageBox.Show("Please provide your security answer.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSecurityAnswer.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(newPassword) Then
            MessageBox.Show("Please enter a new password.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtNewPassword.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(confirmPassword) Then
            MessageBox.Show("Please confirm your new password.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtConfirmPassword.Focus()
            Return
        End If

        ' Check if new password and confirm password match
        If newPassword <> confirmPassword Then
            MessageBox.Show("New Password and Confirm Password do not match.", "Password Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtConfirmPassword.Focus()
            Return
        End If

        ' Check if the password is strong
        If Not IsPasswordStrong(newPassword) Then
            MessageBox.Show("Password is too weak." & vbCrLf & "It must contain at least:" & vbCrLf &
                            "- One uppercase letter" & vbCrLf &
                            "- Three lowercase letters" & vbCrLf &
                            "- One special character (non-letter, non-digit)",
                            "Weak Password", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtNewPassword.Focus()
            Return
        End If

        ' Verify security answer against the database
        If Not VerifySecurityAnswer(userID, securityAnswer) Then
            MessageBox.Show("Incorrect User ID or Security Answer.", "Verification Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return ' Don't proceed if verification fails
        End If

        ' Update the password in the database
        If UpdatePassword(userID, newPassword) Then
            MessageBox.Show("Password updated successfully!" & vbCrLf & "You can now log in with your new password.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ' Optionally clear fields or close form here
            GoToLoginForm() ' Go back to login after success
        Else
            MessageBox.Show("Failed to update password. An error occurred.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    ' Method to check if the password is strong
    Private Function IsPasswordStrong(password As String) As Boolean
        ' Regex checks:
        ' (?=.*[A-Z])        - At least one uppercase letter
        ' (?=(?:.*[a-z]){3}) - At least three lowercase letters (non-capturing group for efficiency)
        ' (?=.*[^a-zA-Z0-9]) - At least one special character
        ' You might also want to add a minimum length check, e.g., .{8,} for at least 8 characters
        Dim strongPasswordPattern As String = "^(?=.*[A-Z])(?=(?:.*[a-z]){3})(?=.*[^a-zA-Z0-9]).{6,}$" ' Added min length 6
        Return Regex.IsMatch(password, strongPasswordPattern)
    End Function

    ' Method to verify the security answer
    Private Function VerifySecurityAnswer(userID As String, securityAnswer As String) As Boolean
        Using conn As New SqlConnection(connectionString)
            ' *** IMPORTANT: Password/Security Answer comparison should ideally be case-sensitive ***
            ' SQL Server collation determines case sensitivity. Default is often case-insensitive (SQL_Latin1_General_CP1_CI_AS).
            ' For sensitive data, use a case-sensitive collation (e.g., SQL_Latin1_General_CP1_CS_AS) for the column
            ' OR use COLLATE in the query if the column isn't CS. Hashing is even better.
            Dim query As String = "SELECT COUNT(*) FROM login WHERE Username = @UserID AND SecurityAnswer = @SecurityAnswer" ' COLLATE SQL_Latin1_General_CP1_CS_AS" ' Optional: Force case-sensitive compare

            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@UserID", userID)
                cmd.Parameters.AddWithValue("@SecurityAnswer", securityAnswer) ' Consider hashing the security answer too
                Try
                    conn.Open()
                    Dim result As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                    Return result > 0 ' True if a matching record exists
                Catch ex As Exception
                    MessageBox.Show("Database error during verification: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False ' Indicate failure
                End Try
            End Using
        End Using
    End Function

    ' Method to update the password
    Private Function UpdatePassword(userID As String, newPassword As String) As Boolean
        Using conn As New SqlConnection(connectionString)
            ' *** SECURITY WARNING: Storing plain text passwords is very insecure! ***
            ' You should HASH the password before storing it.
            ' Example using a simple hash (NOT cryptographically secure for real apps, use bcrypt, Argon2, etc.):
            ' Dim hashedPassword = HashPassword(newPassword) ' Implement a proper hashing function

            Dim query As String = "UPDATE login SET Password = @NewPassword WHERE Username = @UserID"
            Using cmd As New SqlCommand(query, conn)
                ' Store the HASHED password, not the plain text one
                cmd.Parameters.AddWithValue("@NewPassword", newPassword) ' Replace with hashedPassword
                cmd.Parameters.AddWithValue("@UserID", userID)
                Try
                    conn.Open()
                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()
                    Return rowsAffected > 0 ' True if the update was successful (1 row affected)
                Catch ex As Exception
                    MessageBox.Show("Database error during password update: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return False ' Indicate failure
                End Try
            End Using
        End Using
    End Function

    ' Cancel button event
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        GoToLoginForm()
    End Sub

    ' Helper method to navigate back to the Login Form (Form1)
    Private Sub GoToLoginForm()
        ' Try to find an existing instance of Form1
        Dim loginForm As Form1 = Application.OpenForms.OfType(Of Form1)().FirstOrDefault()

        If loginForm Is Nothing Then
            ' If not found, create a new instance
            loginForm = New Form1()
            loginForm.Show() ' Show the new instance
        Else
            ' If found, bring it to the front
            loginForm.WindowState = FormWindowState.Normal ' Ensure it's not minimized
            loginForm.BringToFront()
            loginForm.Show() ' Ensure it's visible if hidden
        End If

        Me.Close() ' Close the current Reset Password form (Form2)
    End Sub

End Class