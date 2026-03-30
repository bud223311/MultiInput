#SingleInstance
Persistent
MultiInputExe := A_WorkingDir "\bin\Debug\net9.0\MultiInput.exe"
ihlistener := InputHook("L1 M")
MainGUI := Gui()
SendToggle := MainGUI.AddButton(,"Send Data")
ReceiveToggle := MainGUI.AddButton(,"Receive Data")
SendIP := MainGUI.AddEdit('Limit15 w80 y7 xm+90')
SendPort := MainGUI.AddEdit('Limit5 w80 y35 xm+90')
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

            StartServer(MultiInputExe, "sender", SendIp.Value, SendPort.Value)

        case "Stop Sending":
            SendToggle.Text := ("Send Data")
            ReceiveToggle.Opt('-Disabled')
            ProcessClose(PID)
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
            ProcessClose(PID)
    }
}

MainGUI.Show('h80 w200')

StartServer(dir, LaunchType, IP, Port)
{
    if IP = ''
    {
        IP := "null"
    }
    Run(dir A_Space A_ScriptHwnd A_Space LaunchType A_Space IP A_Space Port ,,,&PID)
    global PID
/*
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
*/
}

OnExit(MainClose)

MainClose(*){
    if IsSet(PID){
        ProcessClose('ahk_pid ' PID)
    }
    ExitApp
}