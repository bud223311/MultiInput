#SingleInstance
Persistent
MultiInputExe := A_WorkingDir "\bin\Debug\net9.0\MultiInput.exe"
ihlistener := InputHook("L1 M")
MainGUI := Gui()

SendToggle := MainGUI.AddButton('ym+110',"Send Data")
ReceiveToggle := MainGUI.AddButton('ym+110',"Receive Data")
ClearHotkeys := MainGUI.AddButton('ym+110 xm+195', "Clear Hotkeys")

SendIP := MainGUI.AddEdit('Limit15 w90 ym xm+25')
SendPort := MainGUI.AddEdit('Limit5 Number w90 yp+25 xm+25')
MainGUI.AddText('ym+3 xm',"IP:`n`nPort:" )


MainGUI.AddHotkey('xm+190 ym w80')
MainGUI.AddHotkey('xm+190 yp+25 w80')
MainGUI.AddHotkey('xm+190 yp+25 w80')
LastInput := MainGUI.AddText(,"nul")

MainGUI.OnEvent('Close', MainClose)

SendToggle.OnEvent('Click',StartSendData)
StartSendData(*)
{
    BtnText := SendToggle.Text
    switch BtnText{
        case "Send Data":
            MainGUI.Submit(false)
            SendToggle.Text := ("Stop Sending")
            ReceiveToggle.Opt('Disabled')
            if FileExist(MultiInputExe) = ""
            {
                MsgBox("No MultiInput.exe was found ")
                MainClose()
            }

            StartServer(MultiInputExe, "sender", SendIp.Value, SendPort.Value)

        case "Stop Sending":
            SendToggle.Text := ("Send Data")
            ReceiveToggle.Opt('-Disabled')
            try ProcessClose(PID)
    }
}

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

StartServer(dir, LaunchType, IP, Port)
{
    if (Port = "")
    {
        ErrorMessage :=("Port")
    }
    if LaunchType = "sender" && IP = ''
    {
        if LaunchType = "sender" {
            ErrorMessage := ErrorMessage (" IP")
        }
        IP := "null"
    }
    if IsSet(ErrorMessage)
    {
        MsgBox(ErrorMessage " must be specified","Invalid Configuration")
        return
    }
    Run(dir A_Space A_ScriptHwnd A_Space LaunchType A_Space IP A_Space Port ,,,&PID)
    global PID

    if LaunchType = "sender"{
        ihlistener.Start
        while ProcessExist(PID) && LaunchType = "sender"{
            ihlistener.Wait()
            LastInput.Text := ihlistener.Input
            SendInput(ihlistener.Input)
            ihlistener.Start


        }
        ihlistener.Stop
    }

}

OnExit(MainClose)

MainClose(*){
    if IsSet(PID){
        ProcessClose(PID)
    }
    ExitApp
}