using System;
using System.Collections.Generic;

namespace MAPFsimulator
{
    /// <summary>
    /// Interface pro datovou strukturu halda
    /// </summary>
    public interface IHeap<Key, Value> where Key : IComparable
    {
        /// <summary>
        /// vlozeni prvku
        /// </summary>
        void insert(Key k, Value v);
        /// <summary>
        /// vraceni minima
        /// </summary>
        Value getMin();
        /// <summary>
        /// vraceni minima klice
        /// </summary>
        Key getMinKey();
        /// <summary>
        /// odstraneni minimalniho prvku
        /// </summary>
        Value removeMin();
        /// <summary>
        /// pocet prvku v halde
        /// </summary>
        int size();
    }

    /// <summary>
    /// Implementace binarni haldy.
    /// Pouziti v algoritmech CBS a A*.
    /// Jedná se o upravenou implementaci haldy z volne dostupneho projektu dostupneho na adrese http://www.ms.mff.cuni.cz/~truno7am/cvikoUI1/Sokoban.zip.
    /// </summary>
    public class RegularBinaryHeap<Value> : IHeap<int, Value>
    {
        private class TreeNode<Key, TheValue> where Key : IComparable
        {
            public TheValue val { get; set; }
            public Key key { get; set; }
            public int index { get; set; }

            public TreeNode(TheValue value, Key k, int index)
            {
                this.val = value;
                this.key = k;
                this.index = index;
            }

            public override string ToString()
            {
                return "Key :" + key + " index:" + index;
            }
        }

        private IList<TreeNode<int, Value>> tree;

        private bool isRoot(TreeNode<int, Value> t)
        {
            return t.index == 0;
        }

        private bool isLeaf(TreeNode<int, Value> t)
        {
            return getLeftSuccesor(t) == null;
        }

        private TreeNode<int, Value> getPredecessor(TreeNode<int, Value> t)
        {
            return t.index == 0 ? null : tree[(t.index - 1) / 2];
        }

        private TreeNode<int, Value> getLeftSuccesor(TreeNode<int, Value> t)
        {
            int index = t.index * 2 + 1;
            return tree.Count > index ? tree[index] : null;
        }

        private TreeNode<int, Value> getRightSuccesor(TreeNode<int, Value> t)
        {
            int index = t.index * 2 + 2;
            return tree.Count > index ? tree[index] : null;
        }

        /// <summary>
        /// True, pokud halda neobsahuje zadne prvky.
        /// </summary>
        public bool isEmpty()
        {
            return tree.Count == 0;
        }

        private void checkUp(TreeNode<int, Value> node)
        {
            TreeNode<int, Value> current = node,
                predecessor = getPredecessor(current);
            while (!isRoot(current) && current.key < predecessor.key)
            {
                swap(current, predecessor);
                predecessor = getPredecessor(current);
            }
        }

        private void swap(TreeNode<int, Value> current, TreeNode<int, Value> predecessor)
        {
            TreeNode<int, Value> stored = tree[current.index];

            tree[current.index] = tree[predecessor.index];
            tree[predecessor.index] = stored;

            int storedIndex = current.index;
            current.index = predecessor.index;
            predecessor.index = storedIndex;
        }

        private void checkDown(TreeNode<int, Value> node)
        {
            TreeNode<int, Value> current = node,
                succesor = null,
                succesorLeft = getLeftSuccesor(current),
                succesorRight = getRightSuccesor(current);

            if (succesorLeft != null)
            {
                if (succesorRight == null)
                    succesor = succesorLeft;
                else
                    succesor = (succesorLeft.key < succesorRight.key ? succesorLeft : succesorRight);

                while (succesor.key < current.key && !isLeaf(current))
                {
                    swap(current, succesor);

                    succesorLeft = getLeftSuccesor(current);
                    succesorRight = getRightSuccesor(current);
                    if (succesorLeft != null)
                    {
                        if (succesorRight == null)
                            succesor = succesorLeft;
                        else
                            succesor = (succesorLeft.key < succesorRight.key ? succesorLeft : succesorRight);
                    }
                }
            }
        }
        /// <summary>
        /// Vytvori novou binarni haldu.
        /// </summary>
        public RegularBinaryHeap()
        {
            this.tree = new List<TreeNode<int, Value>>();
        }

        #region Heap<int,Value> Members
        /// <summary>
        /// Vlozi prvek v s hodnotou klice k do haldy.
        /// </summary>
        public void insert(int k, Value v)
        {
            TreeNode<int, Value> newNode = new TreeNode<int, Value>(v, k, tree.Count);
            tree.Add(newNode);
            checkUp(newNode);
        }
        /// <summary>
        /// Vraci minimalni prvek haldy.
        /// </summary>
        public Value getMin()
        {
            return (tree.Count > 0 ? tree[0].val : default(Value));
        }
        /// <summary>
        /// Odstrani minimum z haldy.
        /// </summary>
        public Value removeMin()
        {
            Value result = tree[0].val;
            swap(tree[0], tree[tree.Count - 1]);
            tree.RemoveAt(tree.Count - 1);
            if (!isEmpty())
                checkDown(tree[0]);
            return result;
        }

        /// <summary>
        /// Vraci pocet prvku haldy.
        /// </summary>
        /// <returns></returns>
        public int size()
        {
            return tree.Count;
        }
        /// <summary>
        /// Vraci hodnotu nejmensiho klice.
        /// </summary>
        /// <returns></returns>
        public int getMinKey()
        {
            return (tree.Count > 0 ? tree[0].key : -1);
        }

        #endregion
    }

}
