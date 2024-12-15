namespace ContactManagementSystem.UI

open System
open System.Windows.Forms
open System.Drawing
open ContactManagementSystem.Services
open ContactManagementSystem.Models

module MainForm =

    let createForm () =
        // Main Form
        let form = new Form(Text = "Contact Management System", Width = 1000, Height = 700)
        form.StartPosition <- FormStartPosition.CenterScreen
        form.BackColor <- Color.FromArgb(34, 40, 49)  // Dark background
        form.Font <- new Font("Arial", 10.0f, FontStyle.Regular)

        // Title Label
        let titleLabel = new Label(Text = "Contact Management System", Font = new Font("Arial", 16.0f, FontStyle.Bold),
                                   ForeColor = Color.FromArgb(238, 238, 238), Top = 20, Left = 20, AutoSize = true) // Light text

        // Group Box for Contact Details
        let contactGroupBox = new GroupBox(Text = "Contact Details", Top = 60, Left = 20, Width = 450, Height = 250)
        contactGroupBox.Font <- new Font("Arial", 10.0f, FontStyle.Bold)
        contactGroupBox.BackColor <- Color.FromArgb(57, 62, 70)  // Darker background
        contactGroupBox.ForeColor <- Color.FromArgb(238, 238, 238)  // Light text

        // Labels and Textboxes for Contact Details
        let nameLabel = new Label(Text = "Name:", Top = 40, Left = 20, AutoSize = true, ForeColor = Color.FromArgb(238, 238, 238))
        let phoneLabel = new Label(Text = "Phone Number:", Top = 80, Left = 20, AutoSize = true, ForeColor = Color.FromArgb(238, 238, 238))
        let emailLabel = new Label(Text = "Email:", Top = 120, Left = 20, AutoSize = true, ForeColor = Color.FromArgb(238, 238, 238))

        let nameTextBox = new TextBox(Top = 35, Left = 150, Width = 250, BackColor = Color.FromArgb(238, 238, 238), ForeColor = Color.FromArgb(34, 40, 49))
        let phoneTextBox = new TextBox(Top = 75, Left = 150, Width = 250, BackColor = Color.FromArgb(238, 238, 238), ForeColor = Color.FromArgb(34, 40, 49))
        let emailTextBox = new TextBox(Top = 115, Left = 150, Width = 250, BackColor = Color.FromArgb(238, 238, 238), ForeColor = Color.FromArgb(34, 40, 49))

        contactGroupBox.Controls.AddRange([| nameLabel; nameTextBox; phoneLabel; phoneTextBox; emailLabel; emailTextBox |])

        // Buttons Panel
        let buttonPanel = new FlowLayoutPanel(FlowDirection = FlowDirection.LeftToRight, Top = 330, Left = 20, Width = 950, Height = 50)
        buttonPanel.Padding <- new Padding(10)
        buttonPanel.BackColor <- Color.FromArgb(57, 62, 70)  // Darker background

        let addButton = new Button(Text = "Add Contact", Width = 150, Height = 40, BackColor = Color.FromArgb(0, 173, 181), ForeColor = Color.White, FlatStyle = FlatStyle.Flat)
        let searchButton = new Button(Text = "Search Contact", Width = 150, Height = 40, BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White, FlatStyle = FlatStyle.Flat)
        let editButton = new Button(Text = "Edit Contact", Width = 150, Height = 40, BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat)
        let deleteButton = new Button(Text = "Delete Contact", Width = 150, Height = 40, BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White, FlatStyle = FlatStyle.Flat)
        let displayAllButton = new Button(Text = "Display All Contacts", Width = 150, Height = 40, BackColor = Color.FromArgb(23, 162, 184), ForeColor = Color.White, FlatStyle = FlatStyle.Flat)

        buttonPanel.Controls.AddRange([| addButton; searchButton; editButton; deleteButton; displayAllButton |])

        // DataGridView for Contact List
        let contactsGrid = new DataGridView(Top = 400, Left = 20, Width = 950, Height = 250)
        contactsGrid.Columns.Add("Id", "ID") |> ignore
        contactsGrid.Columns.Add("Name", "Name") |> ignore
        contactsGrid.Columns.Add("PhoneNumber", "Phone Number") |> ignore
        contactsGrid.Columns.Add("Email", "Email") |> ignore
        contactsGrid.ReadOnly <- true
        contactsGrid.AutoSizeColumnsMode <- DataGridViewAutoSizeColumnsMode.Fill
        contactsGrid.BackgroundColor <- Color.FromArgb(221, 221, 221)// Light background
        contactsGrid.DefaultCellStyle.BackColor <- Color.FromArgb(221, 221, 221)
        contactsGrid.DefaultCellStyle.ForeColor <- Color.FromArgb(34, 40, 49)
        contactsGrid.DefaultCellStyle.SelectionBackColor <- Color.FromArgb(0, 173, 181)
        contactsGrid.DefaultCellStyle.SelectionForeColor <- Color.FromArgb(221, 221, 221)

        // Mutable State
        let mutable contacts = Map.empty<string, Contact>

        // Add Contact Event
        addButton.Click.Add(fun _ ->
            if String.IsNullOrWhiteSpace(nameTextBox.Text) || 
               String.IsNullOrWhiteSpace(phoneTextBox.Text) || 
               String.IsNullOrWhiteSpace(emailTextBox.Text) then
                MessageBox.Show("All fields are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
            else
                let contact = { Name = nameTextBox.Text; PhoneNumber = phoneTextBox.Text; Email = emailTextBox.Text }
                contacts <- ContactService.addContact contacts contact
                contactsGrid.Rows.Clear()
                contacts |> Map.iter (fun id c -> contactsGrid.Rows.Add(id, c.Name, c.PhoneNumber, c.Email) |> ignore)
                MessageBox.Show("Contact added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
        )

        // Search Contact Event
        searchButton.Click.Add(fun _ ->
            let name = nameTextBox.Text
            let phone = phoneTextBox.Text
            let filteredContacts =
                if not (String.IsNullOrWhiteSpace(name)) then
                    ContactService.searchContactByName contacts name
                elif not (String.IsNullOrWhiteSpace(phone)) then
                    ContactService.searchContactByPhoneNumber contacts phone
                else []
            contactsGrid.Rows.Clear()
            filteredContacts |> List.iter (fun (_, c) -> contactsGrid.Rows.Add("", c.Name, c.PhoneNumber, c.Email) |> ignore)
        )

        // Edit Contact Event
        editButton.Click.Add(fun _ ->
            if contactsGrid.SelectedRows.Count > 0 then
                let selectedRow = contactsGrid.SelectedRows.[0]
                let id = selectedRow.Cells.[0].Value.ToString()
                if contacts.ContainsKey(id) then
                    let updatedContact = { 
                        Name = nameTextBox.Text; 
                        PhoneNumber = phoneTextBox.Text; 
                        Email = emailTextBox.Text 
                    }
                    contacts <- ContactService.editContact contacts id updatedContact
                    selectedRow.Cells.[1].Value <- updatedContact.Name
                    selectedRow.Cells.[2].Value <- updatedContact.PhoneNumber
                    selectedRow.Cells.[3].Value <- updatedContact.Email
                    MessageBox.Show("Contact updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
                else
                    MessageBox.Show("Contact not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        )

        // Delete Contact Event
        deleteButton.Click.Add(fun _ ->
            if contactsGrid.SelectedRows.Count > 0 then
                let selectedRow = contactsGrid.SelectedRows.[0]
                let id = selectedRow.Cells.[0].Value.ToString()
                if contacts.ContainsKey(id) then
                    contacts <- ContactService.deleteContact contacts id
                    contactsGrid.Rows.Remove(selectedRow) |> ignore
                    MessageBox.Show("Contact deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
                else
                    MessageBox.Show("Contact not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        )

        // Display All Contacts Event
        displayAllButton.Click.Add(fun _ ->
            contactsGrid.Rows.Clear()
            if Map.isEmpty contacts then
                MessageBox.Show("No contacts available.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
            else
                contacts
                |> Map.iter (fun id contact -> 
                    contactsGrid.Rows.Add(id, contact.Name, contact.PhoneNumber, contact.Email) |> ignore)
                MessageBox.Show("All contacts displayed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
        )

        // Add Controls to Form
        form.Controls.AddRange([| titleLabel; contactGroupBox; buttonPanel; contactsGrid |])

        // Return Form
        form
