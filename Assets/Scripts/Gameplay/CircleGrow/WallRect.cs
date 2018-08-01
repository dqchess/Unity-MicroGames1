﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircleGrow {
    public class WallRect : Wall {
        // Components
        [SerializeField] private BoxCollider2D myCollider;


		// ----------------------------------------------------------------
		//  Doers
		// ----------------------------------------------------------------
		override public Prop SetSize(Vector2 _size) {
			base.SetSize(_size);
			myCollider.size = _size;
			return this;
		}

    }
}