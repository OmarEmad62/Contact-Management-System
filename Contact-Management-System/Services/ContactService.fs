namespace Contact-Management-System.Services

open System
open Contact-Management-System.Models

module ContactService =

    let addContact (contacts: Map<string, Contact>) contact =
        if Map.exists (fun _ c -> c.Name = contact.Name || c.PhoneNumber = contact.PhoneNumber) contacts then
            printfn "Contact already exists."
            contacts
        else
            let id = Guid.NewGuid().ToString()
            Map.add id contact contacts
            
    let searchContactByName (contacts: Map<string, Contact>) (name: string) =
    contacts
    |> Map.filter (fun _ (c: Contact) -> c.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
    |> Map.toList

let searchContactByPhoneNumber (contacts: Map<string, Contact>) (phoneNumber: string) =
    contacts
    |> Map.filter (fun _ (c: Contact) -> c.PhoneNumber.Contains(phoneNumber, StringComparison.OrdinalIgnoreCase))
    |> Map.toList
    
let editContact (contacts: Map<string, Contact>) id updatedContact =
     if Map.containsKey id contacts then
         Map.add id updatedContact contacts
     else
         printfn "Contact not found."
         contacts
