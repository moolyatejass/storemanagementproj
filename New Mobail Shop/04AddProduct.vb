Imports System.Data.SqlClient
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class InsertMobile

    Dim con As New SqlConnection
    Dim da As New SqlDataAdapter
    Dim com As SqlCommand
    Dim ds As New DataSet
    Dim dr As SqlDataReader

    ' Event handler for Button2 (Clear Form)
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Clear all textboxes and reset comboboxes
        TextBox1.Text = ""
        TextBox2.Text = ""
        TextBox3.Text = ""
        TextBox4.Text = ""
        TextBox5.Text = ""
        TextBox6.Text = ""
        TextBox7.Text = ""
        TextBox8.Text = ""
        TextBox9.Text = ""
        ComboBox2.Text = "--Select--"
        ComboBox3.Text = "--Select--"
        ComboBox4.Text = "--Select--"
        ComboBox5.Text = "--Select--"
    End Sub

    ' Event handler for Button1 (Insert Data)
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            ' Define the connection string
            con.ConnectionString = "Data Source=LOQI\SQLEXPRESS;Initial Catalog=MobileShopManagementSystem;Integrated Security=True"

            ' Define the SQL query with parameters
            Dim query As String = "INSERT INTO mobile (brand, model, price, storage, ram, f_cam, b_cam, warranty, size, date, d_id, d_name, quantity) VALUES (@brand, @model, @price, @storage, @ram, @f_cam, @b_cam, @warranty, @size, @date, @d_id, @d_name, @quantity)"
            com = New SqlCommand(query, con)

            ' Add parameters to prevent SQL injection
            com.Parameters.AddWithValue("@brand", TextBox1.Text)
            com.Parameters.AddWithValue("@model", TextBox2.Text)
            com.Parameters.AddWithValue("@price", TextBox3.Text)
            com.Parameters.AddWithValue("@storage", ComboBox2.Text)
            com.Parameters.AddWithValue("@ram", ComboBox3.Text)
            com.Parameters.AddWithValue("@f_cam", TextBox4.Text)
            com.Parameters.AddWithValue("@b_cam", TextBox5.Text)
            com.Parameters.AddWithValue("@warranty", ComboBox5.Text)
            com.Parameters.AddWithValue("@size", ComboBox4.Text)
            com.Parameters.AddWithValue("@date", TextBox8.Text)
            com.Parameters.AddWithValue("@d_id", TextBox6.Text)
            com.Parameters.AddWithValue("@d_name", TextBox7.Text)
            com.Parameters.AddWithValue("@quantity", TextBox9.Text)

            ' Open the connection and execute the query
            con.Open()
            com.ExecuteNonQuery()
            MsgBox("Mobile Information Inserted Successfully!")

        Catch ex As Exception
            MsgBox("Error: " & ex.Message)
        Finally
            ' Close the connection
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        ' Clear the form after insertion
        Button2_Click(sender, e)
    End Sub

    ' Event handler for Form Load
    Private Sub InsertMobile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Populate ComboBox2 (Storage) with predefined options
        ComboBox2.Items.Clear()
        ComboBox2.Items.Add("--Select--")
        ComboBox2.Items.Add("64GB")
        ComboBox2.Items.Add("128GB")
        ComboBox2.Items.Add("256GB")
        ComboBox2.SelectedIndex = 0 ' Default to "--Select--"

        ' Populate ComboBox3 (RAM) with predefined options
        ComboBox3.Items.Clear()
        ComboBox3.Items.Add("--Select--")
        ComboBox3.Items.Add("4GB")
        ComboBox3.Items.Add("6GB")
        ComboBox3.Items.Add("8GB")
        ComboBox3.SelectedIndex = 0 ' Default to "--Select--"

        ' Populate ComboBox4 (Size) with predefined options
        ComboBox4.Items.Clear()
        ComboBox4.Items.Add("--Select--")
        ComboBox4.Items.Add("5.5 inch")
        ComboBox4.Items.Add("6 inch")
        ComboBox4.Items.Add("6.5 inch")
        ComboBox4.SelectedIndex = 0 ' Default to "--Select--"

        ' Optionally, populate ComboBox5 (Warranty) if needed
        ComboBox5.Items.Clear()
        ComboBox5.Items.Add("--Select--")
        ComboBox5.Items.Add("6 Months")
        ComboBox5.Items.Add("1 Year")
        ComboBox5.Items.Add("2 Years")
        ComboBox5.SelectedIndex = 0 ' Default to "--Select--"
    End Sub

End Class