open System.Windows.Forms
open ContactManagementSystem.UI

[<EntryPoint>]
let main _ =
    let form = MainForm.createForm()
    Application.Run(form)
    0
