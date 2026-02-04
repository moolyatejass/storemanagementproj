Public Class Home

    ' Event handler for Insert Mobile button click
    Private Sub btnInsertMobile_Click(sender As Object, e As EventArgs) Handles btnInsertMobile.Click
        Dim insertMobileForm As New InsertMobile()
        insertMobileForm.ShowDialog()
    End Sub

    ' Event handler for Sell Mobile button click
    Private Sub btnSellMobile_Click(sender As Object, e As EventArgs) Handles btnSellMobile.Click
        Dim sellMobileForm As New SellMobile()
        sellMobileForm.ShowDialog()
    End Sub

    ' Event handler for Search button click
    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim searchForm As New Search()
        searchForm.ShowDialog()
    End Sub

    ' Event handler for Delete button click
    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim deleteForm As New Delete()
        deleteForm.ShowDialog()
    End Sub

    ' Event handler for Update Mobile button click
    Private Sub btnUpdateMobile_Click(sender As Object, e As EventArgs) Handles btnUpdateMobile.Click
        Dim updatePhoneForm As New UpdatePhone()
        updatePhoneForm.ShowDialog()
    End Sub

    ' Event handler for Exit button click
    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Dim form2 As New Form1()
        form2.Show()
        Me.Close() ' Close the current form
    End Sub

    ' Event handler for form load (Optional)
    Private Sub Home_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' You can add initialization code here if needed
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnBillGeneration.Click
        Dim reportForm As New BillGenerationForm()
        reportForm.Show()
        Me.Close()
    End Sub

    Private Sub btnSalesReport_Click(sender As Object, e As EventArgs) Handles btnSalesReport.Click
        Dim reportForm As New SalesReport()
        reportForm.Show()
        Me.Close()
    End Sub

    ' ... (Your existing btnExit_Click and Home_Load) ...


End Class