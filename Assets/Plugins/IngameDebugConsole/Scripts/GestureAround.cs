/*
               #########                       
              ############                     
              #############                    
             ##  ###########                   
            ###  ###### #####                  
            ### #######   ####                 
           ###  ########## ####                
          ####  ########### ####               
         ####   ###########  #####             
        #####   ### ########   #####           
       #####   ###   ########   ######         
      ######   ###  ###########   ######       
     ######   #### ##############  ######      
    #######  #####################  ######     
    #######  ######################  ######    
   #######  ###### #################  ######   
   #######  ###### ###### #########   ######   
   #######    ##  ######   ######     ######   
   #######        ######    #####     #####    
    ######        #####     #####     ####     
     #####        ####      #####     ###      
      #####       ###        ###      #        
        ###       ###        ###               
         ##       ###        ###               
__________#_______####_______####______________

               我们的未来没有BUG                  
* ==============================================================================
* Filename: GestureAround
* Created:  2018/7/28 13:12:05
* Author:   To Harden The Mind
* Purpose:  
* ==============================================================================
*/

namespace IngameDebugConsole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class GestureAround : MonoBehaviour
    {
        public bool bIsClockWise;
        public float fGestureRadius = 50;
        public float fGestureRadiusFix = 10;
        public float fRotateFactor = 500f;//旋转系数
        public DebugLogManager logWin = null;

        private Vector3 _v3AroundCenterPoint;
        private bool _bGesturing;
        private Vector3 _v3LastStarPoint;
        private float _fSampleDisThreshold;
        private List<Vector3> _inputGesturePhases = new List<Vector3>();

        private float _fTotalRotate;

        // Use this for initialization
        void Awake()
        {
            _fSampleDisThreshold = fGestureRadius / 4f;//(2*PI/24),PI~=3
            _fTotalRotate = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _bGesturing = true;
                _v3LastStarPoint = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _bGesturing = false;
            }

            if (_bGesturing)
            {
                //支持一个大概的圆形区,空心处理的小一点,就用修正值了
                if (Vector3.Distance(_v3AroundCenterPoint, Input.mousePosition) > fGestureRadiusFix)
                {
                    var deltaVec = Input.mousePosition - _v3LastStarPoint;
                    if (deltaVec.sqrMagnitude > _fSampleDisThreshold * _fSampleDisThreshold)//超过阈值,记录一下
                    {
                        _inputGesturePhases.Add(deltaVec);
                        if (_inputGesturePhases.Count > 1)
                        {
                            int curCount = _inputGesturePhases.Count;
                            float multiDot = Vector3.Dot(_inputGesturePhases[curCount - 1], _inputGesturePhases[curCount - 2]);
                            Vector3 multiCross = Vector3.Cross(_inputGesturePhases[curCount - 1], _inputGesturePhases[curCount - 2]);
                            if (multiDot <= 0)//画圆只能是锐角
                            {
                                _inputGesturePhases.Clear();
                            }
                            else if (multiCross.z == 0 || (multiCross.z > 0 && !bIsClockWise) || (multiCross.z < 0 && bIsClockWise))//叉积右手法则,顺时针后一条叉前一条,z应该是正,z是0表示平行
                            {
                                _inputGesturePhases.Clear();
                            }
                            else
                            {
                                //通过上面几个条件测试表示是正在画一个圆,可以转动物体了
                                float rotateZ = bIsClockWise ? -1 * fRotateFactor * Time.deltaTime : 1 * fRotateFactor * Time.deltaTime;
                                _fTotalRotate += rotateZ;
                                _v3LastStarPoint = Input.mousePosition;

                                if (Mathf.Abs(_fTotalRotate) > 360)
                                {
                                    //logWin.ToggleDebugWin();
                                    _fTotalRotate = 0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    _inputGesturePhases.Clear();//乱画就清除路径,重新来过
                }
            }
        }
    }
}
