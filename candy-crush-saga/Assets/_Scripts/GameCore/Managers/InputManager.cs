using System;
using Interface;
using UnityEngine;

namespace GameCore.Managers
{
    public class InputManager : MonoBehaviour
    {
        #region Actions

        public Action<INode, INode> OnNodeSwap;

        #endregion

        #region Serialized Fields

        [SerializeField] private LayerMask nodeLayer;

        #endregion

        #region Private Fields

        private INode _firstNode;
        private RaycastHit2D _hit;
        private Camera _mainCamera;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            HandleNodeClick();
            HandleNodeRelease();
        }

        #endregion


        #region Private Methods

        private void HandleNodeClick()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            var node = GetNodeFromInput();
            if (node == null) return;
            _firstNode = node;
        }

        private void HandleNodeRelease()
        {
            if (!Input.GetMouseButtonUp(0)) return;
            var node = GetNodeFromInput();
            if (node == null) { return; }

            if (_firstNode == null) { return; }

            OnNodeSwap?.Invoke(_firstNode, node);
        }

        private INode GetNodeFromInput()
        {
            _hit = Physics2D.Raycast(GetMousePosition(), Vector2.zero, 0.1f, nodeLayer);
            return _hit.collider != null ? _hit.collider.GetComponent<INode>() : null;
        }

        private Vector2 GetMousePosition()
        {
            return new Vector2(_mainCamera.ScreenToWorldPoint(Input.mousePosition).x,
                _mainCamera.ScreenToWorldPoint(Input.mousePosition).y);
        }

        #endregion
    }
}
