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

public class DcmTag : DcmTagKey {
  private IntPtr swigCPtr;

  internal DcmTag(IntPtr cPtr, bool cMemoryOwn) : base(DCMTKPINVOKE.DcmTagUpcast(cPtr), cMemoryOwn) {
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(DcmTag obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  ~DcmTag() {
    Dispose();
  }

  public override void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      DCMTKPINVOKE.delete_DcmTag(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
    base.Dispose();
  }

  public DcmTag() : this(DCMTKPINVOKE.new_DcmTag__SWIG_0(), true) {
  }

  public DcmTag(DcmTagKey akey) : this(DCMTKPINVOKE.new_DcmTag__SWIG_1(DcmTagKey.getCPtr(akey)), true) {
  }

  public DcmTag(ushort g, ushort e) : this(DCMTKPINVOKE.new_DcmTag__SWIG_2(g, e), true) {
  }

  public DcmTag(DcmTagKey akey, DcmVR avr) : this(DCMTKPINVOKE.new_DcmTag__SWIG_3(DcmTagKey.getCPtr(akey), DcmVR.getCPtr(avr)), true) {
  }

  public DcmTag(ushort g, ushort e, DcmVR avr) : this(DCMTKPINVOKE.new_DcmTag__SWIG_4(g, e, DcmVR.getCPtr(avr)), true) {
  }

  public DcmTag(DcmTag tag) : this(DCMTKPINVOKE.new_DcmTag__SWIG_5(DcmTag.getCPtr(tag)), true) {
  }

  public DcmVR setVR(DcmVR avr) {
    return new DcmVR(DCMTKPINVOKE.DcmTag_setVR(swigCPtr, DcmVR.getCPtr(avr)), true);
  }

  public DcmVR getVR() {
    return new DcmVR(DCMTKPINVOKE.DcmTag_getVR(swigCPtr), true);
  }

  public DcmEVR getEVR() {
    return (DcmEVR)DCMTKPINVOKE.DcmTag_getEVR(swigCPtr);
  }

  public string getVRName() {
    return DCMTKPINVOKE.DcmTag_getVRName(swigCPtr);
  }

  public ushort getGTag() {
    return DCMTKPINVOKE.DcmTag_getGTag(swigCPtr);
  }

  public ushort getETag() {
    return DCMTKPINVOKE.DcmTag_getETag(swigCPtr);
  }

  public DcmTagKey getXTag() {
    return new DcmTagKey(DCMTKPINVOKE.DcmTag_getXTag(swigCPtr), true);
  }

  public string getTagName() {
    return DCMTKPINVOKE.DcmTag_getTagName(swigCPtr);
  }

  public string getPrivateCreator() {
    return DCMTKPINVOKE.DcmTag_getPrivateCreator(swigCPtr);
  }

  public void setPrivateCreator(string privCreator) {
    DCMTKPINVOKE.DcmTag_setPrivateCreator(swigCPtr, privCreator);
  }

  public void lookupVRinDictionary() {
    DCMTKPINVOKE.DcmTag_lookupVRinDictionary(swigCPtr);
  }

  public bool isSignable() {
    return DCMTKPINVOKE.DcmTag_isSignable(swigCPtr);
  }

  public bool isUnknownVR() {
    return DCMTKPINVOKE.DcmTag_isUnknownVR(swigCPtr);
  }

  public OFCondition error() {
    return new OFCondition(DCMTKPINVOKE.DcmTag_error(swigCPtr), true);
  }

  public static OFCondition findTagFromName(string name, DcmTag val) {
    return new OFCondition(DCMTKPINVOKE.DcmTag_findTagFromName(name, DcmTag.getCPtr(val)), true);
  }

}

}
