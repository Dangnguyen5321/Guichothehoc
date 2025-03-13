using System;
using SimpleFileBrowser;
using UnityEngine;

public class OpenFileLoadMidi : MonoBehaviour
{
    //public void OnLoadMidiButtonClick()
    //{
    //    // C?u hình b? l?c file
    //    FileBrowser.SetFilters(true, new FileBrowser.Filter("MIDI Files", ".mid", ".midi"));
    //    FileBrowser.SetDefaultFilter(".mid");

    //    // Hi?n th? h?p tho?i ch?n file
    //    FileBrowser.ShowLoadDialog(
    //        (paths) => // paths là m?ng các ???ng d?n ???c ch?n
    //        {
    //            if (paths.Length > 0) // Ki?m tra n?u có file ???c ch?n
    //            {
    //                SongManager.Instance.fileLocation = paths[0]; // L?y file ??u tiên
    //                Debug.Log("File Selected: " + paths[0]);

    //                // T? ??ng load file MIDI sau khi ch?n
    //                SongManager.Instance.LoadSelectedMidi();
    //            }
    //        },
    //        () =>
    //        {
    //            Debug.Log("File selection canceled.");
    //        },
    //        FileBrowser.PickMode.Files);
    //}

    public static OpenFileLoadMidi Instance;
    private Action<string> onFileSelected;

    void Awake()
    {
        Instance = this;
    }

    public void OpenFileBrowser(Action<string> callback)
    {
        onFileSelected = callback; // L?u l?i hàm callback ?? g?i khi ch?n file xong

        // Ch? l?c file MIDI
        FileBrowser.SetFilters(true, new FileBrowser.Filter("MIDI Files", ".mid", ".midi"));
        FileBrowser.SetDefaultFilter(".mid");

        // Hi?n th? h?p tho?i ch?n file
        FileBrowser.ShowLoadDialog(
            (paths) =>
            {
                if (paths.Length > 0)
                {
                    string selectedPath = paths[0];
                    Debug.Log("Selected MIDI File: " + selectedPath);
                    onFileSelected?.Invoke(selectedPath); // G?i callback ?? truy?n file v? SongManager
                }
            },
            () =>
            {
                Debug.Log("File selection canceled.");
            },
            FileBrowser.PickMode.Files
        );
    }

}
