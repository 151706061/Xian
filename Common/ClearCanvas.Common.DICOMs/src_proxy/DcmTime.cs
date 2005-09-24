/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 1.3.24
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace ClearCanvas.Common.DICOM {

using System;
using System.Text;

public class DcmTime : DcmByteString {
  private IntPtr swigCPtr;

  internal DcmTime(IntPtr cPtr, bool cMemoryOwn) : base(DCMTKPINVOKE.DcmTimeUpcast(cPtr), cMemoryOwn) {
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(DcmTime obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  protected DcmTime() : this(IntPtr.Zero, false) {
  }

  ~DcmTime() {
    Dispose();
  }

  public override void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      DCMTKPINVOKE.delete_DcmTime(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
    base.Dispose();
  }

  public DcmTime(DcmTag tag, uint len) : this(DCMTKPINVOKE.new_DcmTime__SWIG_0(DcmTag.getCPtr(tag), len), true) {
  }

  public DcmTime(DcmTag tag) : this(DCMTKPINVOKE.new_DcmTime__SWIG_1(DcmTag.getCPtr(tag)), true) {
  }

  public DcmTime(DcmTime old) : this(DCMTKPINVOKE.new_DcmTime__SWIG_2(DcmTime.getCPtr(old)), true) {
  }

  public override DcmEVR ident() {
    return (DcmEVR)DCMTKPINVOKE.DcmTime_ident(swigCPtr);
  }

  public override OFCondition getOFString(StringBuilder stringValue, uint pos, bool normalize) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getOFString__SWIG_0(swigCPtr, stringValue, pos, normalize), true);
  }

  public override OFCondition getOFString(StringBuilder stringValue, uint pos) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getOFString__SWIG_1(swigCPtr, stringValue, pos), true);
  }

  public OFCondition setCurrentTime(bool seconds, bool fraction) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_setCurrentTime__SWIG_0(swigCPtr, seconds, fraction), true);
  }

  public OFCondition setCurrentTime(bool seconds) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_setCurrentTime__SWIG_1(swigCPtr, seconds), true);
  }

  public OFCondition setCurrentTime() {
    return new OFCondition(DCMTKPINVOKE.DcmTime_setCurrentTime__SWIG_2(swigCPtr), true);
  }

  public OFCondition setOFTime(OFTime timeValue) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_setOFTime(swigCPtr, OFTime.getCPtr(timeValue)), true);
  }

  public OFCondition getOFTime(OFTime timeValue, uint pos, bool supportOldFormat) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getOFTime__SWIG_0(swigCPtr, OFTime.getCPtr(timeValue), pos, supportOldFormat), true);
  }

  public OFCondition getOFTime(OFTime timeValue, uint pos) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getOFTime__SWIG_1(swigCPtr, OFTime.getCPtr(timeValue), pos), true);
  }

  public OFCondition getOFTime(OFTime timeValue) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getOFTime__SWIG_2(swigCPtr, OFTime.getCPtr(timeValue)), true);
  }

  public OFCondition getISOFormattedTime(StringBuilder formattedTime, uint pos, bool seconds, bool fraction, bool createMissingPart, bool supportOldFormat) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTime__SWIG_0(swigCPtr, formattedTime, pos, seconds, fraction, createMissingPart, supportOldFormat), true);
  }

  public OFCondition getISOFormattedTime(StringBuilder formattedTime, uint pos, bool seconds, bool fraction, bool createMissingPart) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTime__SWIG_1(swigCPtr, formattedTime, pos, seconds, fraction, createMissingPart), true);
  }

  public OFCondition getISOFormattedTime(StringBuilder formattedTime, uint pos, bool seconds, bool fraction) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTime__SWIG_2(swigCPtr, formattedTime, pos, seconds, fraction), true);
  }

  public OFCondition getISOFormattedTime(StringBuilder formattedTime, uint pos, bool seconds) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTime__SWIG_3(swigCPtr, formattedTime, pos, seconds), true);
  }

  public OFCondition getISOFormattedTime(StringBuilder formattedTime, uint pos) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTime__SWIG_4(swigCPtr, formattedTime, pos), true);
  }

  public OFCondition getISOFormattedTime(StringBuilder formattedTime) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTime__SWIG_5(swigCPtr, formattedTime), true);
  }

  public static OFCondition getCurrentTime(StringBuilder dicomTime, bool seconds, bool fraction) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getCurrentTime__SWIG_0(dicomTime, seconds, fraction), true);
  }

  public static OFCondition getCurrentTime(StringBuilder dicomTime, bool seconds) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getCurrentTime__SWIG_1(dicomTime, seconds), true);
  }

  public static OFCondition getCurrentTime(StringBuilder dicomTime) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getCurrentTime__SWIG_2(dicomTime), true);
  }

  public static OFCondition getDicomTimeFromOFTime(OFTime timeValue, StringBuilder dicomTime, bool seconds, bool fraction) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getDicomTimeFromOFTime__SWIG_0(OFTime.getCPtr(timeValue), dicomTime, seconds, fraction), true);
  }

  public static OFCondition getDicomTimeFromOFTime(OFTime timeValue, StringBuilder dicomTime, bool seconds) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getDicomTimeFromOFTime__SWIG_1(OFTime.getCPtr(timeValue), dicomTime, seconds), true);
  }

  public static OFCondition getDicomTimeFromOFTime(OFTime timeValue, StringBuilder dicomTime) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getDicomTimeFromOFTime__SWIG_2(OFTime.getCPtr(timeValue), dicomTime), true);
  }

  public static OFCondition getOFTimeFromString(string dicomTime, OFTime timeValue, bool supportOldFormat) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getOFTimeFromString__SWIG_0(dicomTime, OFTime.getCPtr(timeValue), supportOldFormat), true);
  }

  public static OFCondition getOFTimeFromString(string dicomTime, OFTime timeValue) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getOFTimeFromString__SWIG_1(dicomTime, OFTime.getCPtr(timeValue)), true);
  }

  public static OFCondition getISOFormattedTimeFromString(string dicomTime, StringBuilder formattedTime, bool seconds, bool fraction, bool createMissingPart, bool supportOldFormat) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTimeFromString__SWIG_0(dicomTime, formattedTime, seconds, fraction, createMissingPart, supportOldFormat), true);
  }

  public static OFCondition getISOFormattedTimeFromString(string dicomTime, StringBuilder formattedTime, bool seconds, bool fraction, bool createMissingPart) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTimeFromString__SWIG_1(dicomTime, formattedTime, seconds, fraction, createMissingPart), true);
  }

  public static OFCondition getISOFormattedTimeFromString(string dicomTime, StringBuilder formattedTime, bool seconds, bool fraction) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTimeFromString__SWIG_2(dicomTime, formattedTime, seconds, fraction), true);
  }

  public static OFCondition getISOFormattedTimeFromString(string dicomTime, StringBuilder formattedTime, bool seconds) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTimeFromString__SWIG_3(dicomTime, formattedTime, seconds), true);
  }

  public static OFCondition getISOFormattedTimeFromString(string dicomTime, StringBuilder formattedTime) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getISOFormattedTimeFromString__SWIG_4(dicomTime, formattedTime), true);
  }

  public static OFCondition getTimeZoneFromString(string dicomTimeZone, out double timeZone) {
    return new OFCondition(DCMTKPINVOKE.DcmTime_getTimeZoneFromString(dicomTimeZone, out timeZone), true);
  }

}

}
