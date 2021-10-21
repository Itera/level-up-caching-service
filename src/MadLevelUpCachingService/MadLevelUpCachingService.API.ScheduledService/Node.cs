using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadLevelUpCachingService.API.ScheduledService
{
    public class Node
    {
        private bool _isLeaf;
        private bool _isRoot;
        private List<Node> _children;
        private Node _parent;
        private string _name;
        private string _value;

        public Node(string name, string value)
        {
            _name = name;
            _value = value;
            _isLeaf = true;
            _isRoot = true;
        }

        public Node(string name)
        {
            _name = name;
            _isLeaf = false;
            _isRoot = true;
        }

        public Node(string name, string value, Node parent)
        {
            _name = name;
            _value = value;
            _parent = parent;
            _isLeaf = true;
            _isRoot = false;
        }

        public Node(string name, Node parent)
        {
            _name = name;
            _parent = parent;
            _isLeaf = false;
            _isRoot = false;
        }

        public string GetName()
            => _name;

        public void SetName(string name)
        {
            _name = name;
        }

        public string GetValue()
            => _isLeaf ? _value : throw new InvalidNodeTypeException();

        public void SetValue(string value)
        {
            if (!_isLeaf)
            {
                throw new InvalidNodeTypeException();
            }

            _value = value;
        }

        public List<Node> GetChildren()
            => _isLeaf ? throw new InvalidNodeTypeException() : _children;

        public void SetChildren(List<Node> children)
        {
            if (_isLeaf)
            {
                throw new InvalidNodeTypeException();
            }

            _children = children;
        }

        public Node GetParent()
        {
            if (_isRoot)
            {
                throw new InvalidNodeTypeException();
            }

            return _parent;
        }

        public class InvalidNodeTypeException : Exception
        {
        }
    }
}
