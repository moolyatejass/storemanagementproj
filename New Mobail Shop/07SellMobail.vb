Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Public Class SellMobile

    Private ReadOnly connectionString As String = "Data Source=LOQI\SQLEXPRESS;Initial Catalog=MobileShopManagementSystem;Integrated Security=True;TrustServerCertificate=True;"
    Dim mobileList As New DataTable()

    ' --- Form Load Event ---
    Private Sub SellMobile_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializePaymentComboBox()
        ConfigureSaleItemsGrid()
        LoadMobiles()
        ClearForm() ' Disables Add button initially
        TextBox4.Text = Date.Now.ToString("yyyy-MM-dd")
        TextBox1.Focus()
        UpdateGrandTotal()
    End Sub

    ' --- Configure DataGridView Columns ---
    Private Sub ConfigureSaleItemsGrid()
        dgvSaleItems.Columns.Clear()
        dgvSaleItems.AutoGenerateColumns = False

        dgvSaleItems.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "colMobileID", .HeaderText = "ID", .DataPropertyName = "MobileID", .Visible = False}) ' Stores mobile.mobile_id
        dgvSaleItems.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "colBrand", .HeaderText = "Brand", .DataPropertyName = "Brand", .ReadOnly = True})
        dgvSaleItems.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "colModel", .HeaderText = "Model", .DataPropertyName = "Model", .ReadOnly = True})
        dgvSaleItems.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "colQuantity", .HeaderText = "Qty", .DataPropertyName = "Quantity", .ReadOnly = True})
        dgvSaleItems.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "colPrice", .HeaderText = "Price", .DataPropertyName = "Price", .ReadOnly = True})
        dgvSaleItems.Columns.Add(New DataGridViewTextBoxColumn With {
            .Name = "colLineTotal", .HeaderText = "Total", .DataPropertyName = "LineTotal", .ReadOnly = True})

        ' Formatting
        If dgvSaleItems.Columns.Contains("colQuantity") Then
            dgvSaleItems.Columns("colQuantity").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        End If
        If dgvSaleItems.Columns.Contains("colPrice") Then
            dgvSaleItems.Columns("colPrice").DefaultCellStyle.Format = "C2"
            dgvSaleItems.Columns("colPrice").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        End If
        If dgvSaleItems.Columns.Contains("colLineTotal") Then
            dgvSaleItems.Columns("colLineTotal").DefaultCellStyle.Format = "C2"
            dgvSaleItems.Columns("colLineTotal").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        End If

        dgvSaleItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvSaleItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvSaleItems.MultiSelect = False
        dgvSaleItems.AllowUserToAddRows = False
        dgvSaleItems.AllowUserToDeleteRows = False
        dgvSaleItems.ReadOnly = True
    End Sub

    ' --- Initialize Payment ComboBox ---
    Private Sub InitializePaymentComboBox()
        cmbPaymentMethod.Items.Clear()
        cmbPaymentMethod.Items.Add("Cash")
        cmbPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList
        cmbPaymentMethod.SelectedIndex = -1
    End Sub

    ' --- Load Mobiles into ComboBox1 and cache details in mobileList ---
    Private Sub LoadMobiles()
        Using con As New SqlConnection(connectionString)
            Dim sql = "SELECT mobile_id, brand, model, price, quantity FROM mobile WHERE quantity > 0 ORDER BY brand, model"
            Using cmd As New SqlCommand(sql, con)
                Try
                    con.Open()
                    mobileList.Clear()
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(mobileList)

                    Dim comboDataSource As New List(Of Object)
                    comboDataSource.Add(New With {.Mobile_Id = -1, .DisplayText = "-- Select Mobile --"}) ' Placeholder

                    For Each row As DataRow In mobileList.Rows
                        comboDataSource.Add(New With {
                            .Mobile_Id = row.Field(Of Integer)("mobile_id"),
                            .DisplayText = $"{row.Field(Of String)("brand")} - {row.Field(Of String)("model")} (Stock: {row.Field(Of Integer)("quantity")})"
                        })
                    Next

                    ComboBox1.DisplayMember = "DisplayText"
                    ComboBox1.ValueMember = "Mobile_Id"
                    ComboBox1.DataSource = Nothing
                    ComboBox1.DataSource = comboDataSource
                    ComboBox1.SelectedIndex = 0 ' Select the placeholder initially

                Catch ex As Exception
                    MessageBox.Show("Error loading mobiles: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using
    End Sub

    ' --- ComboBox Selection Change - Update Preview in GroupBox1 ---
    Private Sub ComboBox1_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles ComboBox1.SelectionChangeCommitted
        ClearMobileDetailsLabels() ' Disables Add button
        numQuantity.Value = 1

        Console.WriteLine($"DEBUG: Selection Committed. Index={ComboBox1.SelectedIndex}, Value='{ComboBox1.SelectedValue}', Type={If(ComboBox1.SelectedValue Is Nothing, "Nothing", ComboBox1.SelectedValue.GetType().ToString())}")

        If ComboBox1.SelectedValue IsNot Nothing AndAlso TypeOf ComboBox1.SelectedValue Is Integer AndAlso CInt(ComboBox1.SelectedValue) <> -1 Then
            Dim selectedMobileId As Integer = CInt(ComboBox1.SelectedValue)
            Dim mobileRow As DataRow = mobileList.AsEnumerable().FirstOrDefault(Function(r) r.Field(Of Integer)("mobile_id") = selectedMobileId)

            If mobileRow IsNot Nothing Then
                Label7.Text = mobileRow.Field(Of String)("brand")
                Label10.Text = mobileRow.Field(Of String)("model")
                Dim price As Decimal = mobileRow.Field(Of Decimal)("price")
                Label14.Text = price.ToString("C")
                Label15.Text = "N/A"
                Label16.Text = mobileRow.Field(Of Integer)("quantity").ToString()
                btnAddItem.Enabled = True ' Enable add button
            Else
                MessageBox.Show("Could not retrieve details for the selected mobile. List might be outdated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                LoadMobiles()
            End If
        End If
    End Sub

    ' --- Clear Button Handler ---
    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ClearForm()
    End Sub

    ' --- Helper to Clear the Entire Form ---
    Private Sub ClearForm()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        If ComboBox1.Items.Count > 0 Then ComboBox1.SelectedIndex = 0 Else ComboBox1.SelectedIndex = -1
        numQuantity.Value = 1
        dgvSaleItems.Rows.Clear()
        ClearMobileDetailsLabels() ' Disables Add button
        TextBox4.Text = Date.Now.ToString("yyyy-MM-dd")
        cmbPaymentMethod.SelectedIndex = -1
        UpdateGrandTotal()
        TextBox1.Focus()
    End Sub

    ' --- Helper to Clear Preview Labels in GroupBox1 (and disable Add button) ---
    Private Sub ClearMobileDetailsLabels()
        Label7.Text = ""
        Label10.Text = ""
        Label14.Text = ""
        Label15.Text = ""
        Label16.Text = ""
        btnAddItem.Enabled = False ' Disable Add button
    End Sub

    ' --- Add Item Button Click (Handles Add/Update) ---
    Private Sub btnAddItem_Click(sender As Object, e As EventArgs) Handles btnAddItem.Click
        Console.WriteLine($"DEBUG: btnAddItem Clicked. Index={ComboBox1.SelectedIndex}, Value='{ComboBox1.SelectedValue}', Type={If(ComboBox1.SelectedValue Is Nothing, "Nothing", ComboBox1.SelectedValue.GetType().ToString())}")

        If ComboBox1.SelectedIndex <= 0 OrElse ComboBox1.SelectedValue Is Nothing OrElse Not TypeOf ComboBox1.SelectedValue Is Integer OrElse CInt(ComboBox1.SelectedValue) = -1 Then
            MessageBox.Show($"Please select a valid mobile from the list first. (Index: {ComboBox1.SelectedIndex}, Value: '{ComboBox1.SelectedValue}')", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ComboBox1.Focus()
            Return
        End If

        Dim selectedMobileId As Integer = CInt(ComboBox1.SelectedValue)
        Dim quantityToAdd As Integer = CInt(numQuantity.Value)

        If quantityToAdd <= 0 Then
            MessageBox.Show("Please enter a quantity greater than zero.", "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            numQuantity.Focus()
            Return
        End If

        Dim mobileRow As DataRow = mobileList.AsEnumerable().FirstOrDefault(Function(r) r.Field(Of Integer)("mobile_id") = selectedMobileId)
        If mobileRow Is Nothing Then
            MessageBox.Show("Could not find details for the selected mobile. List might be outdated, please refresh.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            LoadMobiles()
            Return
        End If

        Dim existingRow As DataGridViewRow = Nothing
        For Each row As DataGridViewRow In dgvSaleItems.Rows
            If Not row.IsNewRow AndAlso CInt(row.Cells("colMobileID").Value) = selectedMobileId Then
                existingRow = row
                Exit For
            End If
        Next

        Dim availableStock As Integer = mobileRow.Field(Of Integer)("quantity")
        Dim requiredTotalQuantity As Integer
        Dim currentQuantityInGrid As Integer = 0

        If existingRow IsNot Nothing Then
            currentQuantityInGrid = CInt(existingRow.Cells("colQuantity").Value)
            requiredTotalQuantity = currentQuantityInGrid + quantityToAdd
        Else
            requiredTotalQuantity = quantityToAdd
        End If

        If availableStock < requiredTotalQuantity Then
            MessageBox.Show($"Not enough stock for {mobileRow.Field(Of String)("brand")} {mobileRow.Field(Of String)("model")}." & vbCrLf &
                            $"Required: {requiredTotalQuantity}, Available: {availableStock}", "Stock Issue", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim brand As String = mobileRow.Field(Of String)("brand")
        Dim model As String = mobileRow.Field(Of String)("model")
        Dim price As Decimal = mobileRow.Field(Of Decimal)("price")

        If existingRow IsNot Nothing Then
            Dim newQuantity As Integer = currentQuantityInGrid + quantityToAdd
            existingRow.Cells("colQuantity").Value = newQuantity
            existingRow.Cells("colLineTotal").Value = price * newQuantity
            MessageBox.Show($"Updated quantity for {brand} {model} to {newQuantity}.", "Quantity Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Dim lineTotal As Decimal = price * quantityToAdd
            dgvSaleItems.Rows.Add(selectedMobileId, brand, model, quantityToAdd, price, lineTotal)
        End If

        UpdateGrandTotal()

        If ComboBox1.Items.Count > 0 Then ComboBox1.SelectedIndex = 0 Else ComboBox1.SelectedIndex = -1
        numQuantity.Value = 1
        ClearMobileDetailsLabels() ' Disables Add button again
        ComboBox1.Focus()
    End Sub

    ' --- Remove Item Button Click ---
    Private Sub btnRemoveItem_Click(sender As Object, e As EventArgs) Handles btnRemoveItem.Click
        If dgvSaleItems.SelectedRows.Count > 0 Then
            For i As Integer = dgvSaleItems.SelectedRows.Count - 1 To 0 Step -1
                Dim row As DataGridViewRow = dgvSaleItems.SelectedRows(i)
                If Not row.IsNewRow Then
                    dgvSaleItems.Rows.Remove(row)
                End If
            Next
            UpdateGrandTotal()
        ElseIf dgvSaleItems.CurrentRow IsNot Nothing AndAlso Not dgvSaleItems.CurrentRow.IsNewRow Then
            dgvSaleItems.Rows.Remove(dgvSaleItems.CurrentRow)
            UpdateGrandTotal()
        Else
            MessageBox.Show("Please select a row in the list to remove.", "No Row Selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    ' --- Update Grand Total Label ---
    Private Sub UpdateGrandTotal()
        Dim total As Decimal = 0D
        For Each row As DataGridViewRow In dgvSaleItems.Rows
            If Not row.IsNewRow AndAlso row.Cells("colLineTotal").Value IsNot Nothing AndAlso IsNumeric(row.Cells("colLineTotal").Value) Then
                total += CDec(row.Cells("colLineTotal").Value)
            End If
        Next
        lblGrandTotal.Text = total.ToString("C", CultureInfo.CurrentCulture)
    End Sub

    ' --- Save Button Handler (Multi-Item Logic) ---
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not ValidateInputMultiItem() Then Exit Sub

        Dim saleDate As Date
        If Not Date.TryParse(TextBox4.Text, saleDate) Then
            MessageBox.Show("Invalid Sale Date format. Please use YYYY-MM-DD.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) : TextBox4.Focus() : Return
        End If

        Dim paymentMethod As String = cmbPaymentMethod.SelectedItem.ToString()
        Dim grandTotal As Decimal = 0D
        For Each row As DataGridViewRow In dgvSaleItems.Rows
            If Not row.IsNewRow AndAlso row.Cells("colLineTotal").Value IsNot Nothing AndAlso IsNumeric(row.Cells("colLineTotal").Value) Then
                grandTotal += CDec(row.Cells("colLineTotal").Value)
            End If
        Next

        If grandTotal <= 0 Then
            MessageBox.Show("Cannot save a sale with zero or negative total.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) : Return
        End If

        ' --- SQL Statements (Ensure these match your DB Schema) ---
        Dim sqlCustomer As String = "INSERT INTO customer (name, mobile, addr) OUTPUT INSERTED.id VALUES (@name, @mobile, @addr);"
        Dim sqlSalesHeader As String = "INSERT INTO sales (customer_id, sale_date, total_amount, PaymentMethod) OUTPUT INSERTED.SaleID VALUES (@customer_id, @sale_date, @total_amount, @payment_method);"

        ' ============================================================================
        ' *** CORRECTED: Use 'mobile_id' as the column name in sale_items table ***
        Dim sqlSaleItem As String = "INSERT INTO sale_items (SaleID, mobile_id, QuantitySold, PriceAtSale) VALUES (@SaleID, @MobileID, @QuantitySold, @PriceAtSale);"
        '                                                    ^^^^^^^^^ Changed this column name
        ' ============================================================================

        Dim sqlPayment As String = "INSERT INTO payment (SaleID, PaymentMethod, AmountPaid, PaymentTimestamp) VALUES (@SaleID, @PaymentMethod, @AmountPaid, GETDATE());"
        Dim sqlStockUpdate As String = "UPDATE mobile SET quantity = quantity - @quantity WHERE mobile_id = @mobile_id AND quantity >= @quantity;"

        Dim newCustomerId As Integer = -1
        Dim newSaleId As Integer = -1
        Dim transaction As SqlTransaction = Nothing

        Using con As New SqlConnection(connectionString)
            Try
                con.Open()
                transaction = con.BeginTransaction()

                ' --- 1. Insert Customer ---
                Using cmdCustomer As New SqlCommand(sqlCustomer, con, transaction)
                    cmdCustomer.Parameters.AddWithValue("@name", TextBox1.Text.Trim())
                    cmdCustomer.Parameters.AddWithValue("@mobile", TextBox2.Text.Trim())
                    cmdCustomer.Parameters.AddWithValue("@addr", If(String.IsNullOrWhiteSpace(TextBox3.Text), DBNull.Value, TextBox3.Text.Trim()))
                    Dim resultCustId = cmdCustomer.ExecuteScalar()
                    If resultCustId IsNot Nothing AndAlso Not IsDBNull(resultCustId) Then newCustomerId = Convert.ToInt32(resultCustId) Else Throw New Exception("Failed to insert customer.")
                    If newCustomerId <= 0 Then Throw New Exception("Invalid Customer ID.")
                End Using

                ' --- 2. Insert Sales Header ---
                Using cmdSalesHeader As New SqlCommand(sqlSalesHeader, con, transaction)
                    cmdSalesHeader.Parameters.AddWithValue("@customer_id", newCustomerId)
                    cmdSalesHeader.Parameters.AddWithValue("@sale_date", saleDate)
                    cmdSalesHeader.Parameters.AddWithValue("@total_amount", grandTotal)
                    cmdSalesHeader.Parameters.AddWithValue("@payment_method", paymentMethod)
                    Dim resultSaleId = cmdSalesHeader.ExecuteScalar()
                    If resultSaleId IsNot Nothing AndAlso Not IsDBNull(resultSaleId) Then newSaleId = Convert.ToInt32(resultSaleId) Else Throw New Exception("Failed to insert sales header.")
                    If newSaleId <= 0 Then Throw New Exception("Invalid Sale ID.")
                End Using

                ' --- 3. Insert Sale Items & Update Stock ---
                For Each row As DataGridViewRow In dgvSaleItems.Rows
                    If row.IsNewRow Then Continue For
                    Dim mobileId As Integer = CInt(row.Cells("colMobileID").Value) ' Contains mobile.mobile_id
                    Dim quantitySold As Integer = CInt(row.Cells("colQuantity").Value)
                    Dim priceAtSale As Decimal = CDec(row.Cells("colPrice").Value)

                    ' Insert Item (Uses CORRECTED sqlSaleItem)
                    Using cmdItem As New SqlCommand(sqlSaleItem, con, transaction)
                        cmdItem.Parameters.AddWithValue("@SaleID", newSaleId)
                        ' *** Parameter name @MobileID is okay, it receives the mobile_id value ***
                        cmdItem.Parameters.AddWithValue("@MobileID", mobileId)
                        cmdItem.Parameters.AddWithValue("@QuantitySold", quantitySold)
                        cmdItem.Parameters.AddWithValue("@PriceAtSale", priceAtSale)
                        cmdItem.ExecuteNonQuery()
                    End Using

                    ' Update Stock
                    Using cmdStock As New SqlCommand(sqlStockUpdate, con, transaction)
                        cmdStock.Parameters.AddWithValue("@quantity", quantitySold)
                        cmdStock.Parameters.AddWithValue("@mobile_id", mobileId)
                        If cmdStock.ExecuteNonQuery() = 0 Then Throw New Exception($"Insufficient stock for Mobile ID {mobileId} ({row.Cells("colBrand").Value} {row.Cells("colModel").Value}). Sale cancelled.")
                    End Using
                Next

                ' --- 4. Insert Payment ---
                Using cmdPayment As New SqlCommand(sqlPayment, con, transaction)
                    cmdPayment.Parameters.AddWithValue("@SaleID", newSaleId)
                    cmdPayment.Parameters.AddWithValue("@PaymentMethod", paymentMethod)
                    cmdPayment.Parameters.AddWithValue("@AmountPaid", grandTotal)
                    cmdPayment.ExecuteNonQuery()
                End Using

                ' --- 5. Commit ---
                transaction.Commit()
                MessageBox.Show("Sale recorded successfully! Sale ID: " & newSaleId, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

                ' --- 6. Notify Report ---
                Try
                    Dim reportForm As SalesReport = Application.OpenForms.OfType(Of SalesReport).FirstOrDefault()
                    If reportForm IsNot Nothing Then
                        reportForm.LoadSalesReport()
                        reportForm.BringToFront()
                    End If
                Catch notifyEx As Exception
                    Console.WriteLine($"Error notifying Sales Report form: {notifyEx.Message}")
                End Try

                ' --- 7. Refresh UI ---
                LoadMobiles()
                ClearForm()

            Catch ex As Exception
                ' --- 8. Rollback ---
                Try
                    transaction?.Rollback()
                    MessageBox.Show("An error occurred and the transaction was rolled back." & vbCrLf & "Error: " & ex.Message, "Transaction Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Catch rollbackEx As Exception
                    MessageBox.Show("CRITICAL ERROR: Transaction failed AND rollback also failed." & vbCrLf & "Original Error: " & ex.Message & vbCrLf & "Rollback Error: " & rollbackEx.Message, "Critical Rollback Failure", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
                If transaction Is Nothing Then MessageBox.Show("Error saving data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                transaction?.Dispose()
            End Try
        End Using
    End Sub

    ' --- Input Validation Function (Multi-Item) ---
    Private Function ValidateInputMultiItem() As Boolean
        If String.IsNullOrWhiteSpace(TextBox1.Text) Then
            MessageBox.Show("Customer name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) : TextBox1.Focus() : Return False
        End If
        If String.IsNullOrWhiteSpace(TextBox2.Text) Then
            MessageBox.Show("Customer mobile number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) : TextBox2.Focus() : Return False
        End If
        If Not IsNumeric(TextBox2.Text) Then
            MessageBox.Show("Mobile number must contain only digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) : TextBox2.Focus() : Return False
        End If
        If TextBox2.Text.Trim().Length < 7 OrElse TextBox2.Text.Trim().Length > 15 Then
            MessageBox.Show("Please enter a valid mobile number (7-15 digits).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) : TextBox2.Focus() : Return False
        End If

        Dim tempDate As Date
        If String.IsNullOrWhiteSpace(TextBox4.Text) OrElse Not Date.TryParseExact(TextBox4.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, tempDate) Then
            MessageBox.Show("Please enter a valid Sale Date in YYYY-MM-DD format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) : TextBox4.Focus() : Return False
        End If

        If dgvSaleItems.Rows.Count = 0 OrElse (dgvSaleItems.Rows.Count = 1 AndAlso dgvSaleItems.Rows(0).IsNewRow) Then
            MessageBox.Show("You must add at least one mobile item to the sale list.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) : ComboBox1.Focus() : Return False
        End If

        If cmbPaymentMethod.SelectedIndex = -1 Then
            MessageBox.Show("Please select a payment method.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning) : cmbPaymentMethod.Focus() : Return False
        End If

        Return True
    End Function

End Class