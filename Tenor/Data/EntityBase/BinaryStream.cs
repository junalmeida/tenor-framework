using System;
using System.IO;

namespace Tenor.Data
{
    public class BinaryStream : Stream
    {
        private BinaryStream()
        { }

        internal BinaryStream(EntityBase entity, string propName)
            : this(entity, propName, -1)
        {

        }

        internal BinaryStream(EntityBase entity, string propName, long length)
        {
            this.entity = entity;
            this.propName = propName;
            this.length = length;
        }

        internal EntityBase entity;
        internal string propName { get; set; }

        public BinaryStream(byte[] buffer)
        {
            this.buffer = buffer;
            length = buffer.Length;
        }

        bool disposed = false;
        byte[] buffer;
        long length;
        long origin = 0;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                buffer = null;
                disposed = true;
            }
            base.Dispose(disposing);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException(typeof(BinaryStream).FullName);
                //if (buffer == null && length == -1)
                //LoadLength();
                return buffer == null ? length : buffer.LongLength;
            }
        }

        private long _position;
        public override long Position
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException(typeof(BinaryStream).FullName);
                return (long)(this._position - this.origin);
            }
            set
            {
                if (disposed)
                    throw new ObjectDisposedException(typeof(BinaryStream).FullName);
                if (value < 0L)
                {
                    throw new ArgumentOutOfRangeException("value", Util.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
                }
                if (value > 0x7fffffffL)
                {
                    throw new ArgumentOutOfRangeException("value", Util.GetResourceString("ArgumentOutOfRange_MemStreamLength"));
                }
                this._position = this.origin + value;
            }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.disposed)
                throw new ObjectDisposedException(typeof(BinaryStream).FullName);

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", Util.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", Util.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }

            if (Length > -1 && ((Length - offset) < count))
            {
                throw new ArgumentException(Util.GetResourceString("Argument_InvalidOffLen"));
            }

            LoadFromDatabase();

            long num = this.Length - this.Position;
            if (num > count)
            {
                num = count;
            }
            if (num <= 0)
            {
                return 0;
            }

            long num2 = num;
            while (--num2 >= 0)
            {
                buffer[offset + num2] = this.buffer[this.Position + num2];
            }


            this.Position += num;
            return Convert.ToInt32(num);
        }

        private void LoadFromDatabase()
        {
            if (this.buffer == null)
            {
                //go to the database and get the buffer;
                this.buffer = entity.GetPropertyValue(propName, true) as byte[];
            }
        }

        public byte[] ToArray()
        {
            if (this.disposed)
                throw new ObjectDisposedException(typeof(BinaryStream).FullName);
            LoadFromDatabase();
            var res = new byte[this.buffer.LongLength];
            Array.Copy(this.buffer, res, this.buffer.Length);
            return res;
        }


        public override long Seek(long offset, SeekOrigin origin)
        {
            if (this.disposed)
                throw new ObjectDisposedException(typeof(BinaryStream).FullName);
            if (offset > 0x7fffffffL)
            {
                throw new ArgumentOutOfRangeException("offset", Util.GetResourceString("ArgumentOutOfRange_MemStreamLength"));
            }
            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset < 0L)
                        throw new IOException(Util.GetResourceString("IO.IO_SeekBeforeBegin"));
                    this.Position = this.origin + offset;
                    break;

                case SeekOrigin.Current:
                    if ((offset + this.Position) < this.origin)
                        throw new IOException(Util.GetResourceString("IO.IO_SeekBeforeBegin"));

                    this.Position += offset;
                    break;

                case SeekOrigin.End:
                    if ((this.Length + offset) < this.origin)
                        throw new IOException(Util.GetResourceString("IO.IO_SeekBeforeBegin"));
                    this.Position = this.Length + offset;
                    break;
                default:
                    throw new ArgumentException(Util.GetResourceString("Argument_InvalidSeekOrigin"));
            }
            return this.Position;
        }


    }
}
