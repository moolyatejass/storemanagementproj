Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization ' For numeric/date parsing

Public Class UpdatePhone

    ' Consistent connection string
    Private ReadOnly connectionString As String = "Data Source=LOQI\SQLEXPRESS;Initial Catalog=MobileShopManagementSystem;Integrated Security=True;TrustServerCertificate=True;"

    ' Event handler for form load
    Private Sub UpdatePhone_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ClearFormFields() ' Ensure form is clear on load
        ' Add items to ComboBoxes if they are fixed lists and not already added in designer
        ' Example (Check prevents adding duplicates if Load is called multiple times):
        ' If ComboBox2.Items.Count <= 1 Then
        '    ComboBox2.Items.AddRange({"64GB", "128GB", "256GB", "512GB", "1TB"})
        ' End If
        ' If ComboBox3.Items.Count <= 1 Then
        '    ComboBox3.Items.AddRange({"4GB", "6GB", "8GB", "12GB", "16GB"})
        ' End If
        ' If ComboBox4.Items.Count <= 1 Then
        '    ComboBox4.Items.AddRange({"6.1 Inch", "6.5 Inch", "6.7 Inch", "6.9 Inch"})
        ' End If
        ' If ComboBox5.Items.Count <= 1 Then
        '    ComboBox5.Items.AddRange({"1 Year", "2 Years", "None"})
        ' End If
    End Sub

    ' Event handler for Button2 click (Clear Form)
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ClearFormFields()
    End Sub

    ' Helper method to clear fields
    Private Sub ClearFormFields()
        TextBox1.Text = ""  ' Brand
        TextBox2.Text = ""  ' Model
        TextBox3.Text = ""  ' Price
        TextBox4.Text = ""  ' F Cam
        TextBox5.Text = ""  ' B Cam
        TextBox6.Text = ""  ' D ID
        TextBox7.Text = ""  ' D Name
        TextBox8.Text = ""  ' Date
        TextBox9.Text = ""  ' Quantity
        TextBox10.Text = "" ' Mobile ID Input
        ComboBox2.Text = "--Select--" ' Storage
        ComboBox3.Text = "--Select--" ' RAM
        ComboBox4.Text = "--Select--" ' Size
        ComboBox5.Text = "--Select--" ' Warranty
        TextBox10.Focus() ' Set focus back to ID input
    End Sub

    ' Event handler for Button1 click (Update Data)
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        ' --- Basic Validation ---
        Dim mobileIdToUpdate As Integer
        If String.IsNullOrWhiteSpace(TextBox10.Text) OrElse Not Integer.TryParse(TextBox10.Text, mobileIdToUpdate) Then
            MsgBox("Please enter a valid numeric Mobile ID to update.", MsgBoxStyle.Exclamation)
            TextBox10.Focus()
            Return
        End If

        If String.IsNullOrWhiteSpace(TextBox1.Text) Then ' Check if Brand is filled (implies details were loaded)
            MsgBox("Cannot update without mobile details. Please ensure the Mobile ID is correct and details are loaded.", MsgBoxStyle.Exclamation)
            TextBox10.Focus()
            Return
        End If

        Dim price As Decimal
        Dim quantity As Integer
        Dim inventoryDate As Date ' Use a more specific name
        Dim isDateValidOrEmpty As Boolean = True

        If Not Decimal.TryParse(TextBox3.Text, NumberStyles.Any, CultureInfo.CurrentCulture, price) Then ' Allow currency symbol etc.
            MsgBox("Please enter a valid numeric price.", MsgBoxStyle.Exclamation) : TextBox3.Focus() : Return
        End If
        If Not Integer.TryParse(TextBox9.Text, quantity) OrElse quantity < 0 Then
            MsgBox("Please enter a valid non-negative numeric quantity.", MsgBoxStyle.Exclamation) : TextBox9.Focus() : Return
        End If

        ' Only validate date if something is entered
        If Not String.IsNullOrWhiteSpace(TextBox8.Text) Then
            If Not Date.TryParse(TextBox8.Text, inventoryDate) Then
                MsgBox("Please enter a valid date (e.g., YYYY-MM-DD) or leave it blank.", MsgBoxStyle.Exclamation) : TextBox8.Focus() : Return
                isDateValidOrEmpty = False ' Mark as invalid if TryParse fails
            End If
        Else
            ' Date is empty, which is allowed
            isDateValidOrEmpty = True
        End If
        If Not isDateValidOrEmpty Then Return ' Exit if date was invalid

        ' --- End Validation ---


        Using con As New SqlConnection(connectionString)
            ' Use mobile_id in WHERE clause
            Dim query As String = "UPDATE mobile SET brand=@brand, model=@model, price=@price, storage=@storage, ram=@ram, f_cam=@f_cam, b_cam=@b_cam, warranty=@warranty, size=@size, date=@date, d_id=@d_id, d_name=@d_name, quantity=@quantity WHERE mobile_id=@mobile_id"
            Using com As New SqlCommand(query, con)

                ' Add parameters using parsed values where appropriate
                com.Parameters.AddWithValue("@brand", TextBox1.Text.Trim())
                com.Parameters.AddWithValue("@model", TextBox2.Text.Trim())
                com.Parameters.AddWithValue("@price", price) ' Use parsed decimal
                com.Parameters.AddWithValue("@storage", If(ComboBox2.Text = "--Select--", DBNull.Value, CType(ComboBox2.Text, Object)))
                com.Parameters.AddWithValue("@ram", If(ComboBox3.Text = "--Select--", DBNull.Value, CType(ComboBox3.Text, Object)))
                com.Parameters.AddWithValue("@f_cam", If(String.IsNullOrWhiteSpace(TextBox4.Text), DBNull.Value, CType(TextBox4.Text.Trim(), Object)))
                com.Parameters.AddWithValue("@b_cam", If(String.IsNullOrWhiteSpace(TextBox5.Text), DBNull.Value, CType(TextBox5.Text.Trim(), Object)))
                com.Parameters.AddWithValue("@warranty", If(ComboBox5.Text = "--Select--", DBNull.Value, CType(ComboBox5.Text, Object)))
                com.Parameters.AddWithValue("@size", If(ComboBox4.Text = "--Select--", DBNull.Value, CType(ComboBox4.Text, Object)))

                ' Handle Date nullability correctly
                If String.IsNullOrWhiteSpace(TextBox8.Text) Then
                    com.Parameters.AddWithValue("@date", DBNull.Value)
                Else
                    com.Parameters.AddWithValue("@date", inventoryDate) ' Use parsed date
                End If

                com.Parameters.AddWithValue("@d_id", If(String.IsNullOrWhiteSpace(TextBox6.Text), DBNull.Value, CType(TextBox6.Text.Trim(), Object)))
                com.Parameters.AddWithValue("@d_name", If(String.IsNullOrWhiteSpace(TextBox7.Text), DBNull.Value, CType(TextBox7.Text.Trim(), Object)))
                com.Parameters.AddWithValue("@quantity", quantity) ' Use parsed integer
                com.Parameters.AddWithValue("@mobile_id", mobileIdToUpdate) ' Use parsed ID

                Try
                    con.Open()
                    Dim rowsAffected = com.ExecuteNonQuery()
                    If rowsAffected > 0 Then
                        MsgBox("Mobile Record Updated Successfully!", MsgBoxStyle.Information)
                        ClearFormFields() ' Clear form only on successful update
                    Else
                        MsgBox("Mobile ID not found. No record was updated.", MsgBoxStyle.Information)
                    End If

                Catch ex As Exception
                    MsgBox("Error updating record: " & ex.Message, MsgBoxStyle.Critical)
                Finally
                    ' No need to explicitly close connection with Using block
                End Try
            End Using ' com is disposed here
        End Using ' con is disposed here
    End Sub

    ' Event handler for TextBox10 text change (Fetch Mobile Details)
    Private Sub TextBox10_TextChanged(sender As Object, e As EventArgs) Handles TextBox10.TextChanged
        ' Clear detail fields first
        TextBox1.Text = "" : TextBox2.Text = "" : TextBox3.Text = "" : ComboBox2.Text = "--Select--"
        ComboBox3.Text = "--Select--" : TextBox4.Text = "" : TextBox5.Text = "" : ComboBox5.Text = "--Select--"
        ComboBox4.Text = "--Select--" : TextBox8.Text = "" : TextBox6.Text = "" : TextBox7.Text = "" : TextBox9.Text = ""

        Dim mobileIdToFetch As Integer
        ' Only proceed if the text is a valid integer
        If String.IsNullOrWhiteSpace(TextBox10.Text) OrElse Not Integer.TryParse(TextBox10.Text, mobileIdToFetch) Then
            Return
        End If

        Using con As New SqlConnection(connectionString)
            ' Use mobile_id in WHERE clause
            Dim getmob As String = "SELECT brand, model, price, storage, ram, f_cam, b_cam, warranty, size, date, d_id, d_name, quantity FROM mobile WHERE mobile_id = @mobile_id"
            Using com As New SqlCommand(getmob, con)
                com.Parameters.AddWithValue("@mobile_id", mobileIdToFetch)

                Try
                    con.Open()
                    Using dr As SqlDataReader = com.ExecuteReader()
                        If dr.Read() Then
                            ' Use column names for robustness, handle DBNull
                            TextBox1.Text = If(dr.IsDBNull(dr.GetOrdinal("brand")), "", dr.GetString(dr.GetOrdinal("brand")))
                            TextBox2.Text = If(dr.IsDBNull(dr.GetOrdinal("model")), "", dr.GetString(dr.GetOrdinal("model")))
                            TextBox3.Text = If(dr.IsDBNull(dr.GetOrdinal("price")), "", dr.GetDecimal(dr.GetOrdinal("price")).ToString("N2")) ' Format price
                            ComboBox2.Text = If(dr.IsDBNull(dr.GetOrdinal("storage")), "--Select--", dr.GetString(dr.GetOrdinal("storage")))
                            ComboBox3.Text = If(dr.IsDBNull(dr.GetOrdinal("ram")), "--Select--", dr.GetString(dr.GetOrdinal("ram")))
                            TextBox4.Text = If(dr.IsDBNull(dr.GetOrdinal("f_cam")), "", dr.GetString(dr.GetOrdinal("f_cam")))
                            TextBox5.Text = If(dr.IsDBNull(dr.GetOrdinal("b_cam")), "", dr.GetString(dr.GetOrdinal("b_cam")))
                            ComboBox5.Text = If(dr.IsDBNull(dr.GetOrdinal("warranty")), "--Select--", dr.GetString(dr.GetOrdinal("warranty")))
                            ComboBox4.Text = If(dr.IsDBNull(dr.GetOrdinal("size")), "--Select--", dr.GetString(dr.GetOrdinal("size")))
                            TextBox8.Text = If(dr.IsDBNull(dr.GetOrdinal("date")), "", dr.GetDateTime(dr.GetOrdinal("date")).ToString("yyyy-MM-dd")) ' Format date
                            TextBox6.Text = If(dr.IsDBNull(dr.GetOrdinal("d_id")), "", dr.GetString(dr.GetOrdinal("d_id")))
                            TextBox7.Text = If(dr.IsDBNull(dr.GetOrdinal("d_name")), "", dr.GetString(dr.GetOrdinal("d_name")))
                            TextBox9.Text = If(dr.IsDBNull(dr.GetOrdinal("quantity")), "0", dr.GetInt32(dr.GetOrdinal("quantity")).ToString()) ' Default to 0 if null
                        Else
                            ' ID entered but not found, fields remain clear from the top of the method
                            ' Optionally: MsgBox("No mobile found with the given ID.")
                        End If
                    End Using ' dr is disposed here
                Catch ex As Exception
                    MsgBox("Error fetching mobile details: " & ex.Message, MsgBoxStyle.Critical)
                Finally
                    ' No need to explicitly close connection with Using block
                End Try
            End Using ' com is disposed here
        End Using ' con is disposed here
    End Sub

End Class