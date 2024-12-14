namespace ContactManagementSystem.UI

open System
open System.Windows.Forms
open System.Drawing
open ContactManagementSystem.Services
open ContactManagementSystem.Models

module MainForm =
    let createForm () =
        
        let form = new Form(Text = "Contact Management System", Width = 800, Height = 500)
        form.StartPosition <- FormStartPosition.CenterScreen

        
        let nameLabel = new Label(Text = "Name:", Top = 20, Left = 10, AutoSize = true)
        let phoneLabel = new Label(Text = "Phone Number:", Top = 60, Left = 10, AutoSize = true)
        let emailLabel = new Label(Text = "Email:", Top = 100, Left = 10, AutoSize = true)

        
        let nameTextBox = new TextBox(Top = 20, Left = 120, Width = 200)
        let phoneTextBox = new TextBox(Top = 60, Left = 120, Width = 200)
        let emailTextBox = new TextBox(Top = 100, Left = 120, Width = 200)

       
        let addButton = new Button(Text = "Add Contact", Top = 140, Left = 120, Width = 100, BackColor = Color.LightBlue)
        let searchButton = new Button(Text = "Search", Top = 140, Left = 230, Width = 100, BackColor = Color.LightGray)
        let editButton = new Button(Text = "Edit Contact", Top = 180, Left = 120, Width = 100, BackColor = Color.LightGreen)
        let deleteButton = new Button(Text = "Delete Contact", Top = 180, Left = 230, Width = 100, BackColor = Color.LightCoral)

        
        let contactsGrid = new DataGridView(Top = 220, Left = 10, Width = 760, Height = 200)
        contactsGrid.Columns.Add("Id", "ID") |> ignore
        contactsGrid.Columns.Add("Name", "Name") |> ignore
        contactsGrid.Columns.Add("PhoneNumber", "Phone Number") |> ignore
        contactsGrid.Columns.Add("Email", "Email") |> ignore
        contactsGrid.ReadOnly <- true
        contactsGrid.AutoSizeColumnsMode <- DataGridViewAutoSizeColumnsMode.Fill

       
        let mutable contacts = Map.empty<string, Contact>

        
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

        
        form.Controls.AddRange([|
            nameLabel; nameTextBox
            phoneLabel; phoneTextBox
            emailLabel; emailTextBox
            addButton; searchButton; editButton; deleteButton
            contactsGrid
        |])

        
        form
