
using System.Runtime.InteropServices;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Reflection;
namespace Tenor.Security
{
    [ComImport, Guid("947812B3-2AE1-4644-BA86-9E90DED7EC91")]
    internal class SpFileStreamClass
    {
    }

    [ComImport, CoClass(typeof(SpFileStreamClass)), Guid("AF67F125-AB39-4E93-B4A2-CC2E66E182A7")]
    internal interface SpFileStream
    {
        void Open(string fileName, SpeechStreamFileMode mode);

        void Close();
    }

    [ComImport, TypeLibType((short)2), ClassInterface((short)0), Guid("96749377-3391-11D2-9EE3-00C04F797396")]
    internal class SpVoiceClass
    {
    }

    [ComImport, Guid("269316D8-57BD-11D2-9EEE-00C04F797396"), CoClass(typeof(SpVoiceClass))]
    internal interface SpVoice
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x11)]
        ISpeechObjectTokens GetVoices();

        SpObjectToken Voice
        {
            get;
            set;
        }

        object AudioOutputStream
        {
            get;
            set;
        }

        int Rate
        {
            get;
            set;
        }

        void Speak(string text, SpeechVoiceSpeakFlags flags);
        void WaitUntilDone(int seconds);
    }

    [ComImport, TypeLibType((short)0x1040), DefaultMember("Item"), Guid("9285B776-2E7B-4BC0-B53E-580EB6FA967F")]
    public interface ISpeechObjectTokens : IEnumerable
    {
        [DispId(1)]
        int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)]
        SpObjectToken Item([In] int Index);
        //[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "", MarshalTypeRef = typeof(EnumeratorToEnumVariantMarshaler), MarshalCookie = "")]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-4), TypeLibFunc(1)]
        IEnumerator GetEnumerator();
    }

    [ComImport, TypeLibType((short)2), ComConversionLoss, Guid("EF411752-3736-4CB4-9C8C-8EF4CCB58EFE"), ClassInterface((short)0)]
    public class SpObjectTokenClass 
    {
    }

    [ComImport, Guid("C74A3ADC-B727-4500-A84A-B526721C8B8C"), CoClass(typeof(SpObjectTokenClass))]
    public interface SpObjectToken : ISpeechObjectToken
    {
    }

    [ComImport, Guid("C74A3ADC-B727-4500-A84A-B526721C8B8C"), TypeLibType((short)0x1040)]
    public interface ISpeechObjectToken
    {
        [DispId(1)]
        string Id { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
        //[DispId(2)]
        //ISpeechDataKey DataKey { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc(0x40), DispId(2)] get; }
        //[DispId(3)]
        //SpObjectTokenCategory Category { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] get; }
        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        string GetDescription([In, Optional, DefaultParameterValue(0)] int Locale);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc(0x40), DispId(5)]
        void SetId([In, MarshalAs(UnmanagedType.BStr)] string Id, [In, Optional, DefaultParameterValue(""), MarshalAs(UnmanagedType.BStr)] string CategoryID, [In, Optional, DefaultParameterValue(false)] bool CreateIfNotExist);
        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)]
        string GetAttribute([In, MarshalAs(UnmanagedType.BStr)] string AttributeName);
        [return: MarshalAs(UnmanagedType.IUnknown)]
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7)]
        //object CreateInstance([In, Optional, IUnknownConstant, MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter, [In, Optional, DefaultParameterValue(0x17)] SpeechTokenContext ClsContext);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8), TypeLibFunc(0x40)]
        void Remove([In, MarshalAs(UnmanagedType.BStr)] string ObjectStorageCLSID);
        [return: MarshalAs(UnmanagedType.BStr)]
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc(0x40), DispId(9)]
        //string GetStorageFileName([In, MarshalAs(UnmanagedType.BStr)] string ObjectStorageCLSID, [In, MarshalAs(UnmanagedType.BStr)] string KeyName, [In, MarshalAs(UnmanagedType.BStr)] string FileName, [In] SpeechTokenShellFolder Folder);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc(0x40), DispId(10)]
        //void RemoveStorageFileName([In, MarshalAs(UnmanagedType.BStr)] string ObjectStorageCLSID, [In, MarshalAs(UnmanagedType.BStr)] string KeyName, [In] bool DeleteFile);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11), TypeLibFunc(0x40)]
        bool IsUISupported([In, MarshalAs(UnmanagedType.BStr)] string TypeOfUI, [In, Optional, DefaultParameterValue(""), MarshalAs(UnmanagedType.Struct)] ref object ExtraData, [In, Optional, IUnknownConstant, MarshalAs(UnmanagedType.IUnknown)] object Object);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc(0x40), DispId(12)]
        void DisplayUI([In] int hWnd, [In, MarshalAs(UnmanagedType.BStr)] string Title, [In, MarshalAs(UnmanagedType.BStr)] string TypeOfUI, [In, Optional, DefaultParameterValue(""), MarshalAs(UnmanagedType.Struct)] ref object ExtraData, [In, Optional, IUnknownConstant, MarshalAs(UnmanagedType.IUnknown)] object Object);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(13)]
        bool MatchesAttributes([In, MarshalAs(UnmanagedType.BStr)] string Attributes);
    }

 


    internal enum SpeechStreamFileMode
    {
        SSFMOpenForRead = 0,
        SSFMOpenReadWrite = 1,
        SSFMCreate = 2,
        SSFMCreateForWrite = 3
    }

    internal enum SpeechVoiceSpeakFlags
    {
        SVSFDefault = 0,
        SVSFlagsAsync = 1,
        SVSFPurgeBeforeSpeak = 2,
        SVSFIsFilename = 4,
        SVSFIsXML = 8,
        SVSFIsNotXML = 16,
        SVSFPersistXML = 32
    }

}