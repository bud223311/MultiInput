#SingleInstance
Persistent
MultiInputExe := A_WorkingDir "\bin\Debug\net9.0\MultiInput.exe"
ihlistener := InputHook("L1 M")

;If MultiInput doesn't exist, refuse to run
if FileExist(MultiInputExe) = ""
{
    MsgBox("No MultiInput.exe was found ")
    MainClose()
}

MainGUI := Gui()
MainGUI.OnEvent('Close', MainClose)

;Gui launch selection
SendToggle := MainGUI.AddButton('ym+110',"Send Data")
ReceiveToggle := MainGUI.AddButton('ym+110',"Receive Data")
ClearHotkeys := MainGUI.AddButton('ym+110 xm+195', "Clear Hotkeys")

;IP and Port specification
SendIP := MainGUI.AddEdit('Limit15 w90 ym xm+25')
SendPort := MainGUI.AddEdit('Limit5 Number w90 yp+25 xm+25')
MainGUI.AddText('ym+3 xm',"IP:`n`nPort:" )

;Hotkey input boxes
Hotkey1 := MainGUI.AddHotkey('xm+135 ym w80')
Hotkey2 := MainGUI.AddHotkey('xm+135 yp+25 w80')
Hotkey3 := MainGUI.AddHotkey('xm+135 yp+25 w80')
;Visual arrows
MainGUI.AddText('xm+218 ym w4',"->`n`n->`n`n->")

;Inputs to be sent from hotkeys
Key1 := MainGUI.AddEdit('Limit1 xm+230 ym w50')
Key2 := MainGUI.AddEdit('Limit1 xm+230 yp+25 w50')
Key3 := MainGUI.AddEdit('Limit1 xm+230 yp+25 w50')



;debug inputhook monitor
LastInput := MainGUI.AddText(,"nul")

;Send data button function
SendToggle.OnEvent('Click',StartSendData)
StartSendData(*)
{
    ;Use button text for cases, it works..
    BtnText := SendToggle.Text

    FailedHKInit := 0

    switch BtnText{
        case "Send Data":
            MainGUI.Submit(false)

            ;Ensure at least one hotkey has an output
            try Hotkey(Hotkey1.Value, WriteToLog, 'On')
            catch{
                FailedHKInit := +1
            }
            try Hotkey(Hotkey2.Value, WriteToLog, 'On')
            catch{
                FailedHKInit := FailedHKInit +1
            }
            try Hotkey(Hotkey3.Value, WriteToLog, 'On')
            catch{
                FailedHKInit :=(FailedHKInit +1)
            }
            if FailedHKInit > 2{
                MsgBox("At least 1 hotkey must be registered when using Send Data")
                return
            }
            SendToggle.Text := ("Stop Sending")
            ReceiveToggle.Opt('Disabled')
            StartServer(MultiInputExe, "sender", SendIp.Value, SendPort.Value)

        case "Stop Sending":
            try Hotkey(Hotkey1.Value, WriteToLog, 'Off')
            try Hotkey(Hotkey2.Value, WriteToLog, 'Off')
            try Hotkey(Hotkey3.Value, WriteToLog, 'Off')
            
            SendToggle.Text := ("Send Data")
            ReceiveToggle.Opt('-Disabled')
            try ProcessClose(PID)
    }
}

;Receive data button function
ReceiveToggle.OnEvent('Click',StartReceiveData)
StartReceiveData(*)
{
    MainGUI.Submit(false)
    BtnText := ReceiveToggle.Text
    switch BtnText{
        case "Receive Data":
            ReceiveToggle.Text := ("Stop Receiving")
            SendToggle.Opt('Disabled')
            StartServer(MultiInputExe, "receiver",'', SendPort.Value)

        case "Stop Receiving":
            ReceiveToggle.Text := ("Receive Data")
            SendToggle.Opt('-Disabled')
            try ProcessClose(PID)
    }
}

MainGUI.Show('h150 w300')


;Validate inputs, then send arguments when launching MultiInput 
StartServer(dir, LaunchType, IP, Port)
{
    ;If-spam to ensure port and ip are set when necessary
    if LaunchType = 'sender' && IP = '' && Port = ''{
        ErrorMessage := ("Port & IP")
    }
    else if Port = ''{
        ErrorMessage :=("Port")
    }
    else if LaunchType = "sender" && IP = ''{
        ErrorMessage := (" IP")
    }
    if IsSet(ErrorMessage)
    {
        MsgBox(ErrorMessage " must be specified","Invalid Configuration")
        ;Callback as we already changed the button text
        switch LaunchType{
            case "sender":
                StartSendData()
                return
            case "receiver":
                StartReceiveData()
                return
        }
    }
    ;Launch MultiInput with given arguments
    Run(dir A_Space A_ScriptHwnd A_Space LaunchType A_Space IP A_Space Port ,,,&PID)
    global PID

    ;If sender, start listening for inputs //DEBUG
    if LaunchType = "sender"{
        InputMonitor()
    }
}


InputMonitor(*){
    /*
    Thread('Priority', -1)
    ihlistener.Start
    while ProcessExist(PID){
        ihlistener.Wait()
        LastInput.Text := ihlistener.Input
        SendInput(ihlistener.Input)
        ihlistener.Start
    }
    ihlistener.Stop
    */
}


WriteToLog(data){
    MsgBox(data)
}
/*
;Only use the callbacks to pass correct data
hotkey1Call(*){
    LastInput.Text := Key1.Text
    WriteToLog(Key1.Text)
}
hotkey2Call(*){
    LastInput.Text := Key2.Text
    WriteToLog(Key2.Text)
}
hotkey3Call(*){
    LastInput.Text := Key3.Text
    WriteToLog(Key3.Text)
}
*/



OnExit(MainClose)

MainClose(*){
    if IsSet(PID){
        ProcessClose(PID)
    }
    ExitApp
}