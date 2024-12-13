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