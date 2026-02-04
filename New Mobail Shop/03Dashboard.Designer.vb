<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Home
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Home))
        Me.btnInsertMobile = New System.Windows.Forms.Button()
        Me.btnSellMobile = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnUpdateMobile = New System.Windows.Forms.Button()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnSalesReport = New System.Windows.Forms.Button()
        Me.btnBillGeneration = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnInsertMobile
        '
        Me.btnInsertMobile.Location = New System.Drawing.Point(0, 66)
        Me.btnInsertMobile.Name = "btnInsertMobile"
        Me.btnInsertMobile.Size = New System.Drawing.Size(150, 30)
        Me.btnInsertMobile.TabIndex = 0
        Me.btnInsertMobile.Text = "Add Products"
        Me.btnInsertMobile.UseVisualStyleBackColor = True
        '
        'btnSellMobile
        '
        Me.btnSellMobile.Location = New System.Drawing.Point(177, 69)
        Me.btnSellMobile.Name = "btnSellMobile"
        Me.btnSellMobile.Size = New System.Drawing.Size(150, 30)
        Me.btnSellMobile.TabIndex = 1
        Me.btnSellMobile.Text = "Sell Mobile"
        Me.btnSellMobile.UseVisualStyleBackColor = True
        '
        'btnSearch
        '
        Me.btnSearch.Location = New System.Drawing.Point(363, 69)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(150, 30)
        Me.btnSearch.TabIndex = 2
        Me.btnSearch.Text = "Search"
        Me.btnSearch.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.Location = New System.Drawing.Point(544, 69)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(150, 30)
        Me.btnDelete.TabIndex = 3
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnUpdateMobile
        '
        Me.btnUpdateMobile.Location = New System.Drawing.Point(740, 72)
        Me.btnUpdateMobile.Name = "btnUpdateMobile"
        Me.btnUpdateMobile.Size = New System.Drawing.Size(150, 30)
        Me.btnUpdateMobile.TabIndex = 4
        Me.btnUpdateMobile.Text = "Update Mobile"
        Me.btnUpdateMobile.UseVisualStyleBackColor = True
        '
        'btnExit
        '
        Me.btnExit.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnExit.Location = New System.Drawing.Point(778, 8)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(102, 30)
        Me.btnExit.TabIndex = 5
        Me.btnExit.Text = "Logout"
        Me.btnExit.UseVisualStyleBackColor = False
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.btnExit)
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(902, 52)
        Me.Panel1.TabIndex = 6
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Myanmar Text", 14.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.SkyBlue
        Me.Label3.Image = CType(resources.GetObject("Label3.Image"), System.Drawing.Image)
        Me.Label3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Label3.Location = New System.Drawing.Point(12, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(155, 43)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "      cellzone"
        '
        'btnSalesReport
        '
        Me.btnSalesReport.Location = New System.Drawing.Point(544, 128)
        Me.btnSalesReport.Name = "btnSalesReport"
        Me.btnSalesReport.Size = New System.Drawing.Size(138, 31)
        Me.btnSalesReport.TabIndex = 7
        Me.btnSalesReport.Text = "Sales Report"
        Me.btnSalesReport.UseVisualStyleBackColor = True
        '
        'btnBillGeneration
        '
        Me.btnBillGeneration.Location = New System.Drawing.Point(740, 128)
        Me.btnBillGeneration.Name = "btnBillGeneration"
        Me.btnBillGeneration.Size = New System.Drawing.Size(138, 31)
        Me.btnBillGeneration.TabIndex = 8
        Me.btnBillGeneration.Text = "Bill"
        Me.btnBillGeneration.UseVisualStyleBackColor = True
        '
        'Home
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(902, 543)
        Me.Controls.Add(Me.btnBillGeneration)
        Me.Controls.Add(Me.btnSalesReport)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.btnUpdateMobile)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.btnSearch)
        Me.Controls.Add(Me.btnSellMobile)
        Me.Controls.Add(Me.btnInsertMobile)
        Me.Name = "Home"
        Me.Text = "Home"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnInsertMobile As System.Windows.Forms.Button
    Friend WithEvents btnSellMobile As System.Windows.Forms.Button
    Friend WithEvents btnSearch As System.Windows.Forms.Button
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnUpdateMobile As System.Windows.Forms.Button
    Friend WithEvents btnExit As System.Windows.Forms.Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents btnSalesReport As Button
    Friend WithEvents btnBillGeneration As Button
End Class