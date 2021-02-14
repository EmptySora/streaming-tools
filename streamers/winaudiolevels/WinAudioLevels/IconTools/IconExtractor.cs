/*
 *  IconExtractor/IconUtil for .NET
 *  Copyright (C) 2014 Tsuda Kageyu. All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions
 *  are met:
 *
 *   1. Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *   2. Redistributions in binary form must reproduce the above copyright
 *      notice, this list of conditions and the following disclaimer in the
 *      documentation and/or other materials provided with the distribution.
 *
 *  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
 *  TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 *  PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER
 *  OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 *  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 *  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 *  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 *  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WinAudioLevels.IconTools {
    public class IconExtractor : IReadOnlyList<Icon>, IDisposable {
        // Flags for LoadLibraryEx().

        private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        // Resource types for EnumResourceNames().

        private readonly static IntPtr RT_ICON = (IntPtr)3;
        private readonly static IntPtr RT_GROUP_ICON = (IntPtr)14;

        private const int MAX_PATH = 260;

        private IntPtr _library = IntPtr.Zero;
        private int _len = 0;
        private byte[] _dir = null;
        private readonly List<int> _pic_lengths = new List<int>();
        private byte[][] _icon_data = null;   // Binary data of each icon.
        private readonly List<int> _icon_ids = new List<int>(); //the resource ids of each icon.


        /// <summary>
        /// Gets the full path of the associated file.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the count of the icons in the associated file.
        /// </summary>
        public int Count { get; private set; } = 0;

        /// <summary>
        /// Initializes a new instance of the IconExtractor class from the specified file name.
        /// </summary>
        /// <param name="fileName">The file to extract icons from.</param>
        public IconExtractor(string fileName) {
            this.Initialize(fileName);
        }


        /// <summary>
        /// Extracts an icon from the file.
        /// </summary>
        /// <param name="index">Zero based index of the icon to be extracted.</param>
        /// <returns>A System.Drawing.Icon object.</returns>
        /// <remarks>Always returns new copy of the Icon. It should be disposed by the user.</remarks>
        public Icon this[int index] {
            get {
                if (this._disposed_value) {
                    throw new ObjectDisposedException(nameof(IconExtractor));
                }
                if (index < 0 || this.Count <= index) {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                if (this._icon_data[index] == null) {
                    this._icon_data[index] = this.GetIconBytes(index);
                }

                // Create an Icon based on a .ico file in memory.

                using (MemoryStream ms = new MemoryStream(this._icon_data[index])) {
                    return new Icon(ms);
                }
            }
        }

        /// <summary>
        /// Extracts an icon from the file given it's resource identifier.
        /// </summary>
        /// <param name="index">The resource ID of the icon being extracted.</param>
        /// <returns>A System.Drawing.Icon object.</returns>
        /// <remarks>Always returns new copy of the Icon. It should be disposed by the user.</remarks>
        public Icon this[ushort resourceId] {
            get {
                if (!this._icon_ids.Contains(resourceId)) {
                    throw new ArgumentOutOfRangeException(nameof(resourceId));
                }
                return this[this._icon_ids.IndexOf(resourceId)];
            }
        }

        public IEnumerator<Icon> GetEnumerator() {
            if (this._disposed_value) {
                throw new ObjectDisposedException(nameof(IconExtractor));
            }
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        
        /// <summary>
        /// Extracts all the icons from the file.
        /// </summary>
        /// <returns>An array of System.Drawing.Icon objects.</returns>
        /// <remarks>Always returns new copies of the Icons. They should be disposed by the user.</remarks>
        [Obsolete("Do not use this method.",true)]
        public Icon[] GetAllIcons() {
            if (this._disposed_value) {
                throw new ObjectDisposedException(nameof(IconExtractor));
            }
            List<Icon> icons = new List<Icon>();
            for (int i = 0; i < this.Count; ++i) {
                icons.Add(this[i]);
            }

            return icons.ToArray();
        }

        private void Initialize(string fileName) {
            if (this._disposed_value) {
                throw new ObjectDisposedException(nameof(IconExtractor));
            }
            IntPtr hModule = this._library = NativeMethods.LoadLibraryEx(
                fileName ?? throw new ArgumentNullException(nameof(fileName)), 
                IntPtr.Zero, 
                LOAD_LIBRARY_AS_DATAFILE);
            if (hModule == IntPtr.Zero) {
                throw new Win32Exception();
            }
            this.FileName = GetFileName(hModule);
            bool callback(IntPtr h, IntPtr t, IntPtr name, IntPtr l) {
                byte[] dir = this._dir = GetDataFromResource(hModule, RT_GROUP_ICON, name);

                this.Count = BitConverter.ToUInt16(dir, 4);
                this._len = 6 + 16 * this.Count;
                this._pic_lengths.Add(0);
                for (int i = 0; i < this.Count; ++i) {
                    this._len += BitConverter.ToInt32(dir, 6 + 14 * i + 8);
                    ushort id = BitConverter.ToUInt16(dir, 6 + 14 * i + 12);
                    this._icon_ids.Add(id);
                    this._pic_lengths.Add((this._pic_lengths.Count > 0 ? this._pic_lengths.Last() : 0) + GetSizeofResource(hModule, RT_ICON, (IntPtr)id));
                }
                return true;
            }
            NativeMethods.EnumResourceNames(hModule, RT_GROUP_ICON, callback, IntPtr.Zero);
            this._icon_data = new byte[this.Count][]; //initialize the total array but set the values themselves to null.
        }


        /*
        private void OldInitialize(string fileName) {
            if (fileName == null) {
                throw new ArgumentNullException("fileName");
            }

            IntPtr hModule = IntPtr.Zero;
            try {
                hModule = NativeMethods.LoadLibraryEx(fileName, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                if (hModule == IntPtr.Zero) {
                    throw new Win32Exception();
                }

                this.FileName = GetFileName(hModule);

                // Enumerate the icon resource and build .ico files in memory.

                List<byte[]> tmpData = new List<byte[]>();
                List<int> iconResourceIds = new List<int>();

                bool callback(IntPtr h, IntPtr t, IntPtr name, IntPtr l) {
                    // Refer the following URL for the data structures used here:
                    // http://msdn.microsoft.com/en-us/library/ms997538.aspx

                    // RT_GROUP_ICON resource consists of a GRPICONDIR and GRPICONDIRENTRY's.

                    byte[] dir = GetDataFromResource(hModule, RT_GROUP_ICON, name);

                    // Calculate the size of an entire .icon file.

                    int count = BitConverter.ToUInt16(dir, 4);  // GRPICONDIR.idCount
                    int len = 6 + 16 * count;                   // sizeof(ICONDIR) + sizeof(ICONDIRENTRY) * count
                    for (int i = 0; i < count; ++i) {
                        len += BitConverter.ToInt32(dir, 6 + 14 * i + 8);   // GRPICONDIRENTRY.dwBytesInRes
                    }

                    using (BinaryWriter dst = new BinaryWriter(new MemoryStream(len))) {
                        // Copy GRPICONDIR to ICONDIR.

                        dst.Write(dir, 0, 6);

                        int picOffset = 6 + 16 * count; // sizeof(ICONDIR) + sizeof(ICONDIRENTRY) * count

                        for (int i = 0; i < count; ++i) {
                            // Load the picture.

                            ushort id = BitConverter.ToUInt16(dir, 6 + 14 * i + 12);    // GRPICONDIRENTRY.nID
                            byte[] pic = GetDataFromResource(hModule, RT_ICON, (IntPtr)id);
                            iconResourceIds.Add(id);

                            // Copy GRPICONDIRENTRY to ICONDIRENTRY.

                            dst.Seek(6 + 16 * i, SeekOrigin.Begin);

                            dst.Write(dir, 6 + 14 * i, 8);  // First 8bytes are identical.
                            dst.Write(pic.Length);          // ICONDIRENTRY.dwBytesInRes
                            dst.Write(picOffset);           // ICONDIRENTRY.dwImageOffset

                            // Copy a picture.

                            dst.Seek(picOffset, SeekOrigin.Begin);
                            dst.Write(pic, 0, pic.Length);

                            picOffset += pic.Length;
                        }

                        tmpData.Add(((MemoryStream)dst.BaseStream).ToArray());
                    }

                    return true;
                }
                NativeMethods.EnumResourceNames(hModule, RT_GROUP_ICON, callback, IntPtr.Zero);

                this._icon_data = tmpData.ToArray();
                this._icon_ids = iconResourceIds;
            } finally {
                if (hModule != IntPtr.Zero) {
                    NativeMethods.FreeLibrary(hModule);
                }
            }
        }
        */
        private static byte[] GetDataFromResource(IntPtr hModule, IntPtr type, IntPtr name) {
            // Load the binary data from the specified resource.

            IntPtr hResInfo = NativeMethods.FindResource(hModule, name, type);
            if (hResInfo == IntPtr.Zero) {
                throw new Win32Exception();
            }

            IntPtr hResData = NativeMethods.LoadResource(hModule, hResInfo);
            if (hResData == IntPtr.Zero) {
                throw new Win32Exception();
            }

            IntPtr pResData = NativeMethods.LockResource(hResData);
            if (pResData == IntPtr.Zero) {
                throw new Win32Exception();
            }

            uint size = NativeMethods.SizeofResource(hModule, hResInfo);
            if (size == 0) {
                throw new Win32Exception();
            }

            byte[] buf = new byte[size];
            Marshal.Copy(pResData, buf, 0, buf.Length);

            return buf;
        }
        private static int GetSizeofResource(IntPtr hModule, IntPtr type, IntPtr name) {
            // Load the binary data from the specified resource.

            IntPtr handle = NativeMethods.FindResource(hModule, name, type);
            if (handle == IntPtr.Zero) {
                throw new Win32Exception();
            }
            /*
            IntPtr hResData = NativeMethods.LoadResource(hModule, handle);
            if (hResData == IntPtr.Zero) {
                throw new Win32Exception();
            }

            IntPtr pResData = NativeMethods.LockResource(hResData);
            if (pResData == IntPtr.Zero) {
                throw new Win32Exception();
            }
            //can we actually remove these calls safely?
            */
            uint size = NativeMethods.SizeofResource(hModule, handle);
            if (size == 0) {
                throw new Win32Exception();
            }

            return (int)size;
        }

        private static string GetFileName(IntPtr hModule) {
            // Alternative to GetModuleFileName() for the module loaded with
            // LOAD_LIBRARY_AS_DATAFILE option.

            // Get the file name in the format like:
            // "\\Device\\HarddiskVolume2\\Windows\\System32\\shell32.dll"

            string fileName;
            {
                StringBuilder buf = new StringBuilder(MAX_PATH);
                int len = NativeMethods.GetMappedFileName(
                    NativeMethods.GetCurrentProcess(), hModule, buf, buf.Capacity);
                if (len == 0) {
                    throw new Win32Exception();
                }

                fileName = buf.ToString();
            }

            // Convert the device name to drive name like:
            // "C:\\Windows\\System32\\shell32.dll"

            for (char c = 'A'; c <= 'Z'; ++c) {
                string drive = c + ":";
                StringBuilder buf = new StringBuilder(MAX_PATH);
                int len = NativeMethods.QueryDosDevice(drive, buf, buf.Capacity);
                if (len == 0) {
                    continue;
                }

                string devPath = buf.ToString();
                if (fileName.StartsWith(devPath)) {
                    return drive + fileName.Substring(devPath.Length);
                }
            }

            return fileName;
        }


        private byte[] GetIconBytes(int index) {
            if (this._disposed_value) {
                throw new ObjectDisposedException(nameof(IconExtractor));
            }
            using (BinaryWriter dst = new BinaryWriter(new MemoryStream(this._len))) {
                byte[] pic = GetDataFromResource(this._library, RT_ICON, (IntPtr)(ushort)this._icon_ids[index]);
                int picOffset = 6 + 16 * this.Count + this._pic_lengths[index];
                dst.Write(this._dir, 0, 6);

                dst.Seek(6 + 16 * index, SeekOrigin.Begin);
                dst.Write(this._dir, 6 + 14 * index, 8);
                dst.Write(pic.Length);
                dst.Write(picOffset);

                dst.Seek(picOffset, SeekOrigin.Begin);
                dst.Write(pic, 0, pic.Length);

                return ((MemoryStream)dst.BaseStream).ToArray();
            }
        }
        public class Enumerator : IEnumerator<Icon> {
            private readonly IconExtractor _extractor = null;
            private int _index = -1;
            internal Enumerator(IconExtractor ext) {
                this._extractor = ext;
            }
            public Icon Current {
                get {
                    //_icon_data
                    if (this._extractor._icon_data[this._index] == null) {
                        this._extractor._icon_data[this._index] = this._extractor.GetIconBytes(this._index);
                    }
                    return this._extractor[this._index];
                }
            }

            object IEnumerator.Current => this.Current;

            public bool MoveNext() {
                return ++this._index < this._extractor.Count;
            }

            public void Reset() {
                this._index = -1;
            }

            #region IDisposable Support
            private bool _disposed_value = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing) {
                if (!this._disposed_value) {
                    if (disposing) {
                        // TODO: dispose managed state (managed objects).
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    this._disposed_value = true;
                }
            }
            public void Dispose() {
                this.Dispose(true);
            }
            #endregion

        }

        #region IDisposable Support
        private bool _disposed_value = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!this._disposed_value) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                if (this._library != IntPtr.Zero) {
                    NativeMethods.FreeLibrary(this._library);
                }
                this._icon_data = null;
                this._icon_ids.Clear();

                this._disposed_value = true;
            }
        }


        ~IconExtractor() {
            this.Dispose(false);
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
//according to SHDefExtractIconA(LPCSTR pszIconFile, int iIndex, UINT uFlags, HICON* phiconLarge, HICON* phiconSmall, UINT nIconSize)
//negative number is resource ID
//positive number refers to the nth icon in the resource file (0-indexed)