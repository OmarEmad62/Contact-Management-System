namespace ContactManagementSystem.Services

open System
open ContactManagementSystem.Models

module ContactService =

    let addContact (contacts: Map<string, Contact>) contact =
        if Map.exists (fun _ c -> c.Name = contact.Name || c.PhoneNumber = contact.PhoneNumber) contacts then
            printfn "Contact already exists."
            contacts
        else
            let id = Guid.NewGuid().ToString()
            Map.add id contact contacts

    let searchContactByName (contacts: Map<string, Contact>) (name: string) =
        match name with
        | "" -> printfn "Search query is empty."; []
        | _ -> 
            contacts
            |> Map.filter (fun _ (c: Contact) -> c.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            |> Map.toList

    let searchContactByPhoneNumber (contacts: Map<string, Contact>) (phoneNumber: string) =
        match phoneNumber with
        | "" -> printfn "Search query is empty."; []
        | _ ->
            contacts
            |> Map.filter (fun _ (c: Contact) -> c.PhoneNumber.Contains(phoneNumber, StringComparison.OrdinalIgnoreCase))
            |> Map.toList

    let editContact (contacts: Map<string, Contact>) id updatedContact =
        match Map.tryFind id contacts with
        | Some _ -> Map.add id updatedContact contacts
        | None -> printfn "Contact not found."; contacts

    let deleteContact (contacts: Map<string, Contact>) id =
        match Map.tryFind id contacts with
        | Some _ -> Map.remove id contacts
        | None -> printfn "Contact not found."; contacts

    let displayContacts (contacts: Map<string, Contact>) =
        if Map.isEmpty contacts then
            printfn "No contacts available."
        else
            printfn "%-20s %-15s %-25s" "Name" "Phone Number" "Email"
            printfn "%-20s %-15s %-25s" (String.replicate 20 "-") (String.replicate 15 "-") (String.replicate 25 "-")
            contacts
            |> Map.iter (fun _ contact ->
                printfn "%-20s %-15s %-25s" contact.Name contact.PhoneNumber contact.Email)