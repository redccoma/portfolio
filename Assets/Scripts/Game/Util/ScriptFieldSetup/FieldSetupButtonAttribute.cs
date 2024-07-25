/*
 * 객체에 스크립트를 할당후, 함수에 FieldSetupButton 어트리뷰트를 추가하기 위해 선언된 코드.
 */

using UnityEngine;
using System;

namespace Game.Util.ScriptFieldSetup
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FieldSetupButtonAttribute : PropertyAttribute
    {
        public string Name;

        public FieldSetupButtonAttribute(string name = "")
        {
            Name = name;
        }
    }
}