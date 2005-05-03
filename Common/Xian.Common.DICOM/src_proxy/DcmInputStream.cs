/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 1.3.24
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace Xian.Common.DICOM {

using System;
using System.Text;

public class DcmInputStream : IDisposable {
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal DcmInputStream(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(DcmInputStream obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  protected DcmInputStream() : this(IntPtr.Zero, false) {
  }

  ~DcmInputStream() {
    Dispose();
  }

  public virtual void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      DCMTKPINVOKE.delete_DcmInputStream(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
  }

  public virtual bool good() {
    return DCMTKPINVOKE.DcmInputStream_good(swigCPtr);
  }

  public virtual OFCondition status() {
    return new OFCondition(DCMTKPINVOKE.DcmInputStream_status(swigCPtr), true);
  }

  public virtual bool eos() {
    return DCMTKPINVOKE.DcmInputStream_eos(swigCPtr);
  }

  public virtual uint avail() {
    return DCMTKPINVOKE.DcmInputStream_avail(swigCPtr);
  }

  public virtual uint read(SWIGTYPE_p_void buf, uint buflen) {
    return DCMTKPINVOKE.DcmInputStream_read(swigCPtr, SWIGTYPE_p_void.getCPtr(buf), buflen);
  }

  public virtual uint skip(uint skiplen) {
    return DCMTKPINVOKE.DcmInputStream_skip(swigCPtr, skiplen);
  }

  public virtual uint tell() {
    return DCMTKPINVOKE.DcmInputStream_tell(swigCPtr);
  }

  public virtual OFCondition installCompressionFilter(E_StreamCompression filterType) {
    return new OFCondition(DCMTKPINVOKE.DcmInputStream_installCompressionFilter(swigCPtr, (int)filterType), true);
  }

  public virtual DcmInputStreamFactory newFactory() {
    IntPtr cPtr = DCMTKPINVOKE.DcmInputStream_newFactory(swigCPtr);
    return (cPtr == IntPtr.Zero) ? null : new DcmInputStreamFactory(cPtr, false);
  }

  public virtual void mark() {
    DCMTKPINVOKE.DcmInputStream_mark(swigCPtr);
  }

  public virtual void putback() {
    DCMTKPINVOKE.DcmInputStream_putback(swigCPtr);
  }

}

}
