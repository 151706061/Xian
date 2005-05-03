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

public class DcmXfer : IDisposable {
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal DcmXfer(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(DcmXfer obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  protected DcmXfer() : this(IntPtr.Zero, false) {
  }

  ~DcmXfer() {
    Dispose();
  }

  public virtual void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      DCMTKPINVOKE.delete_DcmXfer(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
  }

  public DcmXfer(E_TransferSyntax xfer) : this(DCMTKPINVOKE.new_DcmXfer__SWIG_0((int)xfer), true) {
  }

  public DcmXfer(string xferName_xferID) : this(DCMTKPINVOKE.new_DcmXfer__SWIG_1(xferName_xferID), true) {
  }

  public DcmXfer(DcmXfer newXfer) : this(DCMTKPINVOKE.new_DcmXfer__SWIG_2(DcmXfer.getCPtr(newXfer)), true) {
  }

  public E_TransferSyntax getXfer() {
    return (E_TransferSyntax)DCMTKPINVOKE.DcmXfer_getXfer(swigCPtr);
  }

  public E_ByteOrder getByteOrder() {
    return (E_ByteOrder)DCMTKPINVOKE.DcmXfer_getByteOrder(swigCPtr);
  }

  public string getXferName() {
    return DCMTKPINVOKE.DcmXfer_getXferName(swigCPtr);
  }

  public string getXferID() {
    return DCMTKPINVOKE.DcmXfer_getXferID(swigCPtr);
  }

  public bool isLittleEndian() {
    return DCMTKPINVOKE.DcmXfer_isLittleEndian(swigCPtr);
  }

  public bool isBigEndian() {
    return DCMTKPINVOKE.DcmXfer_isBigEndian(swigCPtr);
  }

  public bool isImplicitVR() {
    return DCMTKPINVOKE.DcmXfer_isImplicitVR(swigCPtr);
  }

  public bool isExplicitVR() {
    return DCMTKPINVOKE.DcmXfer_isExplicitVR(swigCPtr);
  }

  public bool isEncapsulated() {
    return DCMTKPINVOKE.DcmXfer_isEncapsulated(swigCPtr);
  }

  public bool isNotEncapsulated() {
    return DCMTKPINVOKE.DcmXfer_isNotEncapsulated(swigCPtr);
  }

  public uint getJPEGProcess8Bit() {
    return DCMTKPINVOKE.DcmXfer_getJPEGProcess8Bit(swigCPtr);
  }

  public uint getJPEGProcess12Bit() {
    return DCMTKPINVOKE.DcmXfer_getJPEGProcess12Bit(swigCPtr);
  }

  public E_StreamCompression getStreamCompression() {
    return (E_StreamCompression)DCMTKPINVOKE.DcmXfer_getStreamCompression(swigCPtr);
  }

  public uint sizeofTagHeader(DcmEVR evr) {
    return DCMTKPINVOKE.DcmXfer_sizeofTagHeader(swigCPtr, (int)evr);
  }

}

}
