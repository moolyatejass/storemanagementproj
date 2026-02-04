Imports System.Data
Imports System.Data.SqlClient

Public Class SalesReport

    ' --- Centralized Connection String (Consistent with other forms) ---
    Private ReadOnly connectionString As String = "Data Source=LOQI\SQLEXPRESS;Initial Catalog=MobileShopManagementSystem;Integrated Security=True;TrustServerCertificate=True;"

    ' --- Form Load Event: Load the report initially ---
    Private Sub SalesReport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSalesReport() ' Load data when the form opens
    End Sub

    ' --- Refresh Button Click Event ---
    Private Sub btnLoadReport_Click(sender As Object, e As EventArgs) Handles btnLoadReport.Click
        LoadSalesReport() ' Reload data when refresh is clicked
    End Sub

    ' --- Close Button Click Event ---
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close() ' Close this form
    End Sub

    ' --- Method to Load Sales Data into the DataGridView ---
    Public Sub LoadSalesReport()
        Dim dtReport As New DataTable()

        ' ========================================================================
        ' *** CORRECTED SQL QUERY for MULTI-ITEM Sales Report ***
        ' Joins sales (header), customer, sale_items (detail), and mobile
        ' ========================================================================
        Dim sql As String = "SELECT " &
                        "    s.SaleID, " &                 ' Use the correct PK name for sales table
                        "    s.sale_date AS SaleDate, " &
                        "    c.name AS CustomerName, " &
                        "    m.brand AS MobileBrand, " &
                        "    m.model AS MobileModel, " &
                        "    si.QuantitySold, " &          ' Get Quantity from sale_items table
                        "    si.PriceAtSale, " &           ' Get Price from sale_items table
                        "    (si.QuantitySold * si.PriceAtSale) AS LineTotal " & ' Calculate line total here
                        "FROM " &
                        "    sales s " &                   ' Alias for sales table
                        "JOIN " &
                        "    customer c ON s.customer_id = c.id " & ' Join sales to customer
                        "JOIN " &
                        "    sale_items si ON s.SaleID = si.SaleID " & ' Join sales to sale_items (use correct FK names)
                        "JOIN " &
                        "    mobile m ON si.mobile_id = m.mobile_id " & ' Join sale_items to mobile (use correct PK/FK names)
                        "ORDER BY " &
                        "    s.sale_date DESC, s.SaleID DESC, si.SaleItemID ASC" ' Order by date, sale, then item

        Using con As New SqlConnection(connectionString)
            Using cmd As New SqlCommand(sql, con)
                Try
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(dtReport)

                    ' Bind the results to the DataGridView
                    dgvSalesReport.DataSource = Nothing ' Unbind first
                    dgvSalesReport.DataSource = dtReport

                    ' Format the DataGridView (Optional formatting)
                    FormatSalesReportGrid()

                Catch exSql As SqlException
                    MessageBox.Show($"SQL Error loading sales report: {exSql.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    dgvSalesReport.DataSource = Nothing ' Clear grid on error
                Catch ex As Exception
                    MessageBox.Show($"An unexpected error occurred loading the report: {ex.Message}", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    dgvSalesReport.DataSource = Nothing ' Clear grid on error
                End Try
            End Using
        End Using
    End Sub

    ' --- Optional Helper Method to Format the DataGridView ---
    Private Sub FormatSalesReportGrid()
        If dgvSalesReport.DataSource Is Nothing Then Exit Sub ' Exit if no data

        Try ' Add Try..Catch for formatting robustness
            ' Example: Format the Date column
            If dgvSalesReport.Columns.Contains("SaleDate") Then
                dgvSalesReport.Columns("SaleDate").DefaultCellStyle.Format = "yyyy-MM-dd"
            End If

            ' Example: Format the Currency columns
            If dgvSalesReport.Columns.Contains("PriceAtSale") Then
                dgvSalesReport.Columns("PriceAtSale").DefaultCellStyle.Format = "C2"
                dgvSalesReport.Columns("PriceAtSale").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            End If
            If dgvSalesReport.Columns.Contains("LineTotal") Then
                dgvSalesReport.Columns("LineTotal").DefaultCellStyle.Format = "C2"
                dgvSalesReport.Columns("LineTotal").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                dgvSalesReport.Columns("LineTotal").HeaderText = "Item Total" ' Rename calculated column
            End If

            ' Example: Align Quantity
            If dgvSalesReport.Columns.Contains("QuantitySold") Then
                dgvSalesReport.Columns("QuantitySold").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                dgvSalesReport.Columns("QuantitySold").HeaderText = "Qty" ' Rename column
            End If

            ' Example: Rename columns for clarity
            If dgvSalesReport.Columns.Contains("SaleID") Then dgvSalesReport.Columns("SaleID").HeaderText = "Sale ID"
            If dgvSalesReport.Columns.Contains("SaleDate") Then dgvSalesReport.Columns("SaleDate").HeaderText = "Date"
            If dgvSalesReport.Columns.Contains("CustomerName") Then dgvSalesReport.Columns("CustomerName").HeaderText = "Customer"
            If dgvSalesReport.Columns.Contains("MobileBrand") Then dgvSalesReport.Columns("MobileBrand").HeaderText = "Brand"
            If dgvSalesReport.Columns.Contains("MobileModel") Then dgvSalesReport.Columns("MobileModel").HeaderText = "Model"
            If dgvSalesReport.Columns.Contains("PriceAtSale") Then dgvSalesReport.Columns("PriceAtSale").HeaderText = "Unit Price"


            ' Auto-size columns based on content and header
            dgvSalesReport.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)

        Catch ex As Exception
            Console.WriteLine($"Error formatting Sales Report Grid: {ex.Message}")
            ' Optionally show a less intrusive error or just log it
        End Try

    End Sub


End Class