// These classes encapsulates COM access.
// We decided not to strongly reference Tenor with Interop since this feature 
// does note have massive use.
#if !MONO
#pragma warning disable
using System.Runtime.InteropServices;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Reflection;
using System;

namespace Tenor.Security
{
    #region Streams
    [ComImport, TypeLibType((short)2), Guid("947812B3-2AE1-4644-BA86-9E90DED7EC91"), ClassInterface((short)0)]
    internal class SpFileStreamClass : ISpeechFileStream, SpFileStream, ISpStream
    {
        // Methods
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void BindToFile(ref ushort pszFileName, SPFILEMODE eMode, ref Guid pFormatId, ref WaveFormatEx pWaveFormatEx, ulong ullEventInterest);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Clone([MarshalAs(UnmanagedType.Interface)] out IStream ppstm);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x65)]
        public virtual extern void Close();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Commit([In] uint grfCommitFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetBaseStream([MarshalAs(UnmanagedType.Interface)] ref IStream ppStream);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetFormat(ref Guid pguidFormatId, IntPtr ppCoMemWaveFormatEx);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void ISpStream_Close();
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void LockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(100)]
        public virtual extern void Open([In, MarshalAs(UnmanagedType.BStr)] string FileName, [In, Optional, DefaultParameterValue(SpeechStreamFileMode.SSFMOpenForRead)] SpeechStreamFileMode FileMode, [In, Optional, DefaultParameterValue(false)] bool DoEvents);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        public virtual extern int Read([MarshalAs(UnmanagedType.Struct)] out object Buffer, [In] int NumberOfBytes);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void RemoteCopyTo([In, MarshalAs(UnmanagedType.Interface)] IStream pstm, [In] _ULARGE_INTEGER cb, out _ULARGE_INTEGER pcbRead, out _ULARGE_INTEGER pcbWritten);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void RemoteRead(out byte pv, [In] uint cb, out uint pcbRead);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void RemoteSeek([In] _LARGE_INTEGER dlibMove, [In] uint dwOrigin, out _ULARGE_INTEGER plibNewPosition);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void RemoteWrite([In] ref byte pv, [In] uint cb, out uint pcbWritten);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Revert();
        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        public virtual extern object Seek([In, MarshalAs(UnmanagedType.Struct)] object Position, [In, Optional, DefaultParameterValue(SpeechStreamSeekPositionType.SSSPTRelativeToStart)] SpeechStreamSeekPositionType Origin);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void SetBaseStream([MarshalAs(UnmanagedType.Interface)] IStream pStream, ref Guid rguidFormat, ref WaveFormatEx pWaveFormatEx);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void SetSize([In] _ULARGE_INTEGER libNewSize);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void Stat(out tagSTATSTG pstatstg, [In] uint grfStatFlag);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void UnlockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        public virtual extern int Write([In, MarshalAs(UnmanagedType.Struct)] object Buffer);

        // Properties
        [DispId(1)]
        public virtual extern SpAudioFormat Format { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] set; }

        [DispId(1)]
        extern SpAudioFormat ISpeechFileStream.Format { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] set; }
    }

 



    [ComImport, CoClass(typeof(SpFileStreamClass)), Guid("AF67F125-AB39-4E93-B4A2-CC2E66E182A7")]
    internal interface SpFileStream : ISpeechFileStream
    {
    }

    [ComImport, Guid("AF67F125-AB39-4E93-B4A2-CC2E66E182A7"), TypeLibType((short)0x1040)]
    internal interface ISpeechFileStream : ISpeechBaseStream
    {
        [DispId(1)]
        SpAudioFormat Format { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] set; }
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        int Read([MarshalAs(UnmanagedType.Struct)] out object Buffer, [In] int NumberOfBytes);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        int Write([In, MarshalAs(UnmanagedType.Struct)] object Buffer);
        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        object Seek([In, MarshalAs(UnmanagedType.Struct)] object Position, [In, Optional, DefaultParameterValue(SpeechStreamSeekPositionType.SSSPTRelativeToStart)] SpeechStreamSeekPositionType Origin);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(100)]
        void Open([In, MarshalAs(UnmanagedType.BStr)] string FileName, [In, Optional, DefaultParameterValue(SpeechStreamFileMode.SSFMOpenForRead)] SpeechStreamFileMode FileMode, [In, Optional, DefaultParameterValue(false)] bool DoEvents);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x65)]
        void Close();
    }





    [ComImport, InterfaceType((short)1), Guid("12E3CCA9-7518-44C5-A5E7-BA5A79CB929E"), TypeLibType((short)0x200)]
    public interface ISpStream : ISpStreamFormat
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoteRead(out byte pv, [In] uint cb, out uint pcbRead);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoteWrite([In] ref byte pv, [In] uint cb, out uint pcbWritten);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void RemoteSeek([In] _LARGE_INTEGER dlibMove, [In] uint dwOrigin, out _ULARGE_INTEGER plibNewPosition);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void SetSize([In] _ULARGE_INTEGER libNewSize);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void RemoteCopyTo([In, MarshalAs(UnmanagedType.Interface)] IStream pstm, [In] _ULARGE_INTEGER cb, out _ULARGE_INTEGER pcbRead, out _ULARGE_INTEGER pcbWritten);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Commit([In] uint grfCommitFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Revert();
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void LockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void UnlockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void Stat(out tagSTATSTG pstatstg, [In] uint grfStatFlag);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Clone([MarshalAs(UnmanagedType.Interface)] out IStream ppstm);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFormat(ref Guid pguidFormatId, IntPtr ppCoMemWaveFormatEx);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void SetBaseStream([MarshalAs(UnmanagedType.Interface)] IStream pStream, ref Guid rguidFormat, ref WaveFormatEx pWaveFormatEx);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetBaseStream([MarshalAs(UnmanagedType.Interface)] ref IStream ppStream);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void BindToFile(ref ushort pszFileName, SPFILEMODE eMode, ref Guid pFormatId, ref WaveFormatEx pWaveFormatEx, ulong ullEventInterest);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Close();
    }



    [ComImport, Guid("6450336F-7D49-4CED-8097-49D6DEE37294"), TypeLibType((short)0x1040)]
    internal interface ISpeechBaseStream
    {
        [DispId(1)]
        SpAudioFormat Format { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] set; }
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        int Read([MarshalAs(UnmanagedType.Struct)] out object Buffer, [In] int NumberOfBytes);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        int Write([In, MarshalAs(UnmanagedType.Struct)] object Buffer);
        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        object Seek([In, MarshalAs(UnmanagedType.Struct)] object Position, [In, Optional, DefaultParameterValue(SpeechStreamSeekPositionType.SSSPTRelativeToStart)] SpeechStreamSeekPositionType Origin);
    }


    [ComImport, Guid("0000000C-0000-0000-C000-000000000046"), InterfaceType((short)1)]
    public interface IStream : ISequentialStream
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoteRead(out byte pv, [In] uint cb, out uint pcbRead);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoteWrite([In] ref byte pv, [In] uint cb, out uint pcbWritten);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void RemoteSeek([In] _LARGE_INTEGER dlibMove, [In] uint dwOrigin, out _ULARGE_INTEGER plibNewPosition);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void SetSize([In] _ULARGE_INTEGER libNewSize);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void RemoteCopyTo([In, MarshalAs(UnmanagedType.Interface)] IStream pstm, [In] _ULARGE_INTEGER cb, out _ULARGE_INTEGER pcbRead, out _ULARGE_INTEGER pcbWritten);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Commit([In] uint grfCommitFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Revert();
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void LockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void UnlockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void Stat(out tagSTATSTG pstatstg, [In] uint grfStatFlag);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Clone([MarshalAs(UnmanagedType.Interface)] out IStream ppstm);
    }

    [ComImport, Guid("0C733A30-2A1C-11CE-ADE5-00AA0044773D"), InterfaceType((short)1)]
    public interface ISequentialStream
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoteRead(out byte pv, [In] uint cb, out uint pcbRead);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoteWrite([In] ref byte pv, [In] uint cb, out uint pcbWritten);
    }

    [ComImport, TypeLibType((short)0x200), ComConversionLoss, Guid("BED530BE-2606-4F4D-A1C0-54C5CDA5566F"), InterfaceType((short)1)]
    public interface ISpStreamFormat : IStream
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoteRead(out byte pv, [In] uint cb, out uint pcbRead);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoteWrite([In] ref byte pv, [In] uint cb, out uint pcbWritten);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void RemoteSeek([In] _LARGE_INTEGER dlibMove, [In] uint dwOrigin, out _ULARGE_INTEGER plibNewPosition);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void SetSize([In] _ULARGE_INTEGER libNewSize);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void RemoteCopyTo([In, MarshalAs(UnmanagedType.Interface)] IStream pstm, [In] _ULARGE_INTEGER cb, out _ULARGE_INTEGER pcbRead, out _ULARGE_INTEGER pcbWritten);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Commit([In] uint grfCommitFlags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Revert();
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void LockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void UnlockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void Stat(out tagSTATSTG pstatstg, [In] uint grfStatFlag);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Clone([MarshalAs(UnmanagedType.Interface)] out IStream ppstm);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFormat(ref Guid pguidFormatId, IntPtr ppCoMemWaveFormatEx);
    }

    #endregion

    #region Voices


    [ComImport, TypeLibType((short)2), ClassInterface((short)0), Guid("96749377-3391-11D2-9EE3-00C04F797396")]
    internal class SpVoiceClass
    {
    }

    [ComImport, Guid("269316D8-57BD-11D2-9EEE-00C04F797396"), CoClass(typeof(SpVoiceClass)), TypeLibType((short) 0x1040)]
    internal interface SpVoice
    {
        [DispId(1)]
        ISpeechVoiceStatus Status { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
        [DispId(2)]
        SpObjectToken Voice { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] set; }
        [DispId(3)]
        SpObjectToken AudioOutput { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] set; }
        [DispId(4)]
        ISpeechBaseStream AudioOutputStream { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)] set; }
        [DispId(5)]
        int Rate { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)] set; }
        [DispId(6)]
        int Volume { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)] set; }
        [DispId(7)]
        bool AllowAudioOutputFormatChangesOnNextSet { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7), TypeLibFunc(0x40)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc(0x40), DispId(7)] set; }
        [DispId(8)]
        object EventInterests { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)] set; }
        [DispId(9)]
        object Priority { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(9)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(9)] set; }
        [DispId(10)]
        object AlertBoundary { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(10)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(10)] set; }
        [DispId(11)]
        int SynchronousSpeakTimeout { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)] set; }
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(12)]
        int Speak([In, MarshalAs(UnmanagedType.BStr)] string Text, [In, Optional, DefaultParameterValue(SpeechVoiceSpeakFlags.SVSFDefault)] SpeechVoiceSpeakFlags Flags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(13)]
        int SpeakStream([In, MarshalAs(UnmanagedType.Interface)] ISpeechBaseStream Stream, [In, Optional, DefaultParameterValue(SpeechVoiceSpeakFlags.SVSFDefault)] SpeechVoiceSpeakFlags Flags);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(14)]
        void Pause();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(15)]
        void Resume();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x10)]
        int Skip([In, MarshalAs(UnmanagedType.BStr)] string Type, [In] int NumItems);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x11)]
        ISpeechObjectTokens GetVoices([In, Optional, DefaultParameterValue(""), MarshalAs(UnmanagedType.BStr)] string RequiredAttributes, [In, Optional, DefaultParameterValue(""), MarshalAs(UnmanagedType.BStr)] string OptionalAttributes);
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x12)]
        ISpeechObjectTokens GetAudioOutputs([In, Optional, DefaultParameterValue(""), MarshalAs(UnmanagedType.BStr)] string RequiredAttributes, [In, Optional, DefaultParameterValue(""), MarshalAs(UnmanagedType.BStr)] string OptionalAttributes);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x13)]
        bool WaitUntilDone([In] int msTimeout);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(20), TypeLibFunc(0x40)]
        int SpeakCompleteEvent();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x15)]
        bool IsUISupported([In, MarshalAs(UnmanagedType.BStr)] string TypeOfUI, [In, Optional, DefaultParameterValue(""), MarshalAs(UnmanagedType.Struct)] ref object ExtraData);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x16)]
        void DisplayUI([In] int hWndParent, [In, MarshalAs(UnmanagedType.BStr)] string Title, [In, MarshalAs(UnmanagedType.BStr)] string TypeOfUI, [In, Optional, DefaultParameterValue(""), MarshalAs(UnmanagedType.Struct)] ref object ExtraData);
    }

    [ComImport, TypeLibType((short)0x1040), Guid("8BE47B07-57F6-11D2-9EEE-00C04F797396")]
    internal interface ISpeechVoiceStatus
    {
        //[DispId(1)]
        //int CurrentStreamNumber { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
        //[DispId(2)]
        //int LastStreamNumberQueued { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; }
        //[DispId(3)]
        //int LastHResult { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] get; }
        //[DispId(4)]
        //SpeechRunState RunningState { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)] get; }
        //[DispId(5)]
        //int InputWordPosition { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)] get; }
        //[DispId(6)]
        //int InputWordLength { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)] get; }
        //[DispId(7)]
        //int InputSentencePosition { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7)] get; }
        //[DispId(8)]
        //int InputSentenceLength { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)] get; }
        //[DispId(9)]
        //string LastBookmark { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(9)] get; }
        //[DispId(10)]
        //int LastBookmarkId { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc(0x40), DispId(10)] get; }
        //[DispId(11)]
        //short PhonemeId { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)] get; }
        //[DispId(12)]
        //short VisemeId { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(12)] get; }
    }

    #endregion


    [ComImport, Guid("E6E9C590-3E18-40E3-8299-061F98BDE7C7"), TypeLibType((short)0x1040)]
    internal interface SpAudioFormat
    {
        //[DispId(1)]
        //SpeechAudioFormatType Type { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] set; }
        //[DispId(2)]
        //string Guid { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2), TypeLibFunc(0x40)] get; [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc(0x40), DispId(2)] set; }
        //[return: MarshalAs(UnmanagedType.Interface)]
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc(0x40), DispId(3)]
        //SpWaveFormatEx GetWaveFormatEx();
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), TypeLibFunc(0x40), DispId(4)]
        //void SetWaveFormatEx([In, MarshalAs(UnmanagedType.Interface)] SpWaveFormatEx WaveFormatEx);
    }



    [ComImport, TypeLibType((short)0x1040), DefaultMember("Item"), Guid("9285B776-2E7B-4BC0-B53E-580EB6FA967F")]
    internal interface ISpeechObjectTokens //: IEnumerable
    {
        [DispId(1)]
        int Count { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; }
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0)]
        SpObjectToken Item([In] int Index);
        //[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "", MarshalTypeRef = typeof(System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler), MarshalCookie = "")]
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(-4), TypeLibFunc(1)]
        //IEnumerator GetEnumerator();
    }


    [ComImport, TypeLibType((short)2), ComConversionLoss, Guid("EF411752-3736-4CB4-9C8C-8EF4CCB58EFE"), ClassInterface((short)0)]
    internal class SpObjectTokenClass 
    {
    }

    [ComImport, Guid("C74A3ADC-B727-4500-A84A-B526721C8B8C"), CoClass(typeof(SpObjectTokenClass))]
    internal interface SpObjectToken : ISpeechObjectToken
    {
    }

    [ComImport, Guid("C74A3ADC-B727-4500-A84A-B526721C8B8C"), TypeLibType((short)0x1040)]
    internal interface ISpeechObjectToken
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



    [ComImport, Guid("5FB7EF7D-DFF4-468A-B6B7-2FCBD188F994"), TypeLibType((short)2), ClassInterface((short)0)]
    internal class SpMemoryStreamClass //: ISpeechMemoryStream, SpMemoryStream//, ISpStream
    {
        // Methods
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void BindToFile(ref ushort pszFileName, SPFILEMODE eMode, ref Guid pFormatId, ref WaveFormatEx pWaveFormatEx, ulong ullEventInterest);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void Clone([MarshalAs(UnmanagedType.Interface)] out IStream ppstm);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void Close();
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void Commit([In] uint grfCommitFlags);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void GetBaseStream([MarshalAs(UnmanagedType.Interface)] ref IStream ppStream);
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x65)]
        //public virtual extern object GetData();
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void GetFormat(ref Guid pguidFormatId, IntPtr ppCoMemWaveFormatEx);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void LockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        //public virtual extern int Read([MarshalAs(UnmanagedType.Struct)] out object Buffer, [In] int NumberOfBytes);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void RemoteCopyTo([In, MarshalAs(UnmanagedType.Interface)] IStream pstm, [In] _ULARGE_INTEGER cb, out _ULARGE_INTEGER pcbRead, out _ULARGE_INTEGER pcbWritten);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void RemoteRead(out byte pv, [In] uint cb, out uint pcbRead);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void RemoteSeek([In] _LARGE_INTEGER dlibMove, [In] uint dwOrigin, out _ULARGE_INTEGER plibNewPosition);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void RemoteWrite([In] ref byte pv, [In] uint cb, out uint pcbWritten);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void Revert();
        //[return: MarshalAs(UnmanagedType.Struct)]
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        //public virtual extern object Seek([In, MarshalAs(UnmanagedType.Struct)] object Position, [In, Optional, DefaultParameterValue(SpeechStreamSeekPositionType.SSSPTRelativeToCurrentPosition)] SpeechStreamSeekPositionType Origin);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void SetBaseStream([MarshalAs(UnmanagedType.Interface)] IStream pStream, ref Guid rguidFormat, ref WaveFormatEx pWaveFormatEx);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(100)]
        //public virtual extern void SetData([In, MarshalAs(UnmanagedType.Struct)] object Data);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void SetSize([In] _ULARGE_INTEGER libNewSize);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void Stat(out tagSTATSTG pstatstg, [In] uint grfStatFlag);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //public virtual extern void UnlockRegion([In] _ULARGE_INTEGER libOffset, [In] _ULARGE_INTEGER cb, [In] uint dwLockType);
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        //public virtual extern int Write([In, MarshalAs(UnmanagedType.Struct)] object Buffer);

        //// Properties
        //[DispId(1)]
        //public virtual extern SpAudioFormat Format { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] set; }
        //[DispId(1)]
        //public virtual SpAudioFormat SpeechLib.ISpeechMemoryStream.Format { [return: MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] get; [param: In, MarshalAs(UnmanagedType.Interface)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)] set; }
    }

    [ComImport, Guid("EEB14B68-808B-4ABE-A5EA-B51DA7588008"), TypeLibType((short)0x1040)]
    internal interface ISpeechMemoryStream : ISpeechBaseStream
    {
    }

    [ComImport, Guid("EEB14B68-808B-4ABE-A5EA-B51DA7588008"), CoClass(typeof(SpMemoryStreamClass))]
    internal interface SpMemoryStream : ISpeechMemoryStream
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Close();


        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x65)]
        object GetData();
    }


    #region Enums



    internal enum SpeechStreamFileMode
    {
        [TypeLibVar(0x40)]
        SSFMCreate = 2,
        SSFMCreateForWrite = 3,
        SSFMOpenForRead = 0,
        [TypeLibVar(0x40)]
        SSFMOpenReadWrite = 1
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

    internal enum SpeechStreamSeekPositionType
    {
        SSSPTRelativeToStart,
        SSSPTRelativeToCurrentPosition,
        SSSPTRelativeToEnd
    }

    [TypeLibType((short)0x10)]
    internal enum SPFILEMODE
    {
        SPFM_OPEN_READONLY,
        SPFM_OPEN_READWRITE,
        SPFM_CREATE,
        SPFM_CREATE_ALWAYS,
        SPFM_NUM_MODES
    }


    #endregion
}

/*
 * 
 * Make Your Controls Speak with C# .NET
 * by Peter A. Bromberg, Ph.D.
 * 
 */

/*

namespace WaveLib
{
    public class Converter
    {
        public Converter()
        {
        }

        public string[] getInstalledVoices()
        {
            SpVoice speech = new SpVoice();
            ISpeechObjectTokens sot = speech.GetVoices("", "");
            string[] voiceIds = new string[sot.Count];
            for (int i = 0; i < sot.Count; i++)
                voiceIds[i] = sot.Item(i).GetDescription(1033);
            return voiceIds;
        }

        public void TextToWav(string inputText, string filePath, string voiceIdString)
        {
            try
            {
                HttpContext ctx = HttpContext.Current;

                if (ctx != null)
                {
                    DirectoryInfo di = new DirectoryInfo(ctx.Server.MapPath("."));
                    FileInfo[] fi = di.GetFiles("*.wav");
                    foreach (FileInfo f in fi)
                        File.Delete(ctx.Server.MapPath(f.Name));
                }

                SpeechVoiceSpeakFlags SpFlags = SpeechVoiceSpeakFlags.SVSFlagsAsync;
                SpVoice speech = new SpVoice();

                if (voiceIdString != String.Empty)
                {
                    ISpeechObjectTokens sot = speech.GetVoices("", "");
                    string[] voiceIds = new string[sot.Count];
                    for (int i = 0; i < sot.Count; i++)
                    {
                        voiceIds[i] = sot.Item(i).GetDescription(1033);
                        if (voiceIds[i] == voiceIdString)
                            speech.Voice = sot.Item(i);
                    }
                }
                SpeechStreamFileMode SpFileMode = SpeechStreamFileMode.SSFMCreateForWrite;
                SpFileStream SpFileStream = new SpFileStream();
                SpFileStream.Format.Type = SpeechAudioFormatType.SAFT11kHz8BitMono;
                if (!filePath.ToLower().EndsWith(".wav")) filePath += ".wav";
                SpFileStream.Open(filePath, SpFileMode, false);
                speech.AudioOutputStream = SpFileStream;
                speech.Speak(inputText, SpFlags);
                speech.WaitUntilDone(Timeout.Infinite);
                SpFileStream.Close();
            }
            catch
            {
                throw;
            }
        }

        public static void TextToWavPlay(string inputText, string voiceIdString)
        {

            byte[] b = TextToWav(inputText, voiceIdString);
            // play bytes here
            Player m_Player = Player.Instance;
            WaveFormat fmt;
            fmt = new WaveFormat(11000, 8, 1);
            m_Player.SetInput(new MemoryStream(b), fmt);
            m_Player.Start(b.Length);
        }

        public static byte[] TextToWav(string inputText, string voiceIdString)
        {
            byte[] b = null;
            try
            {
                SpeechVoiceSpeakFlags SpFlags = SpeechVoiceSpeakFlags.SVSFlagsAsync;
                SpVoice speech = new SpVoice();
                if (voiceIdString != String.Empty)
                {
                    ISpeechObjectTokens sot = speech.GetVoices("", "");
                    string[] voiceIds = new string[sot.Count];
                    for (int i = 0; i < sot.Count; i++)
                    {
                        voiceIds[i] = sot.Item(i).GetDescription(1033);
                        if (voiceIds[i] == voiceIdString)
                            speech.Voice = sot.Item(i);
                    }
                }

                SpMemoryStream spMemStream = new SpMemoryStream();
                spMemStream.Format.Type = SpeechAudioFormatType.SAFT11kHz8BitMono;
                object buf = new object();
                speech.AudioOutputStream = spMemStream;
                int r = speech.Speak(inputText, SpFlags);
                speech.WaitUntilDone(Timeout.Infinite);
                spMemStream.Seek(0, SpeechStreamSeekPositionType.SSSPTRelativeToStart);
                buf = spMemStream.GetData();
                b = (byte[])buf;
            }
            catch
            {
                throw;
            }
            return b;
        }
    }
}
*/
#pragma warning restore
#endif