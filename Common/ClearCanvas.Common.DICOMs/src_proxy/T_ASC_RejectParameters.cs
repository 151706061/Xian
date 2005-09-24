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

public class T_ASC_RejectParameters : IDisposable {
  private IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal T_ASC_RejectParameters(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static IntPtr getCPtr(T_ASC_RejectParameters obj) {
    return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
  }

  ~T_ASC_RejectParameters() {
    Dispose();
  }

  public virtual void Dispose() {
    if(swigCPtr != IntPtr.Zero && swigCMemOwn) {
      swigCMemOwn = false;
      DCMTKPINVOKE.delete_T_ASC_RejectParameters(swigCPtr);
    }
    swigCPtr = IntPtr.Zero;
    GC.SuppressFinalize(this);
  }

  public T_ASC_RejectParametersResult result {
    set {
      DCMTKPINVOKE.set_T_ASC_RejectParameters_result(swigCPtr, (int)value);
    } 
    get {
      return (T_ASC_RejectParametersResult)DCMTKPINVOKE.get_T_ASC_RejectParameters_result(swigCPtr);
    } 
  }

  public T_ASC_RejectParametersSource source {
    set {
      DCMTKPINVOKE.set_T_ASC_RejectParameters_source(swigCPtr, (int)value);
    } 
    get {
      return (T_ASC_RejectParametersSource)DCMTKPINVOKE.get_T_ASC_RejectParameters_source(swigCPtr);
    } 
  }

  public T_ASC_RejectParametersReason reason {
    set {
      DCMTKPINVOKE.set_T_ASC_RejectParameters_reason(swigCPtr, (int)value);
    } 
    get {
      return (T_ASC_RejectParametersReason)DCMTKPINVOKE.get_T_ASC_RejectParameters_reason(swigCPtr);
    } 
  }

  public T_ASC_RejectParameters() : this(DCMTKPINVOKE.new_T_ASC_RejectParameters(), true) {
  }

}

}
