using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAlgebre
{
    //Interfaces des opérateurs
    public interface IOpGroupe<C>
    {
        C Somme(C a, C b);
        C Zero();
        C Symetrique(C a);
        bool Egalite(C a, C b);
    }
    public interface IOpGroupeAbelien<C>:IOpGroupe<C>
    {

    }
    public interface IOpAnneauUnitaire<C>:IOpGroupeAbelien<C>
    {
        C Produit(C a, C b);
        C Un();
    }
    public interface IOpAnneauCommutatif<C> : IOpAnneauUnitaire<C>
    {

    }
    public interface IOpCorps<C> : IOpAnneauCommutatif<C>
    {
        C Inverse(C a);
        bool NonNul(C a);
    }
    public interface IOpEspaceVect<E,K>
    {
        //Opérateurs sur les scalaires
        IOpCorps<K> OpK();
        //Opérateurs sur les vecteurs
        IOpGroupeAbelien<E> OpE();
        //Opérateur mixte
        E Point(K k, E e);
    }
    public interface IOpAlgebre<E,K> : IOpEspaceVect<E,K>
    {
        E Fois(E a, E b);
    }
    public interface IOpEspacePreHilbertien<E,K> : IOpEspaceVect<E, K>
    {
        K Scalaire(E a, E b);
        bool SupZero(K a);
    }
    

    //Testeurs d'axiomes pour les opérateurs
    public struct TesteurAxiomes<C>
    {
        public bool TesterAxiomes(IOpGroupe<C> op, C x1, C x2, C x3)
        {
            //Associativité
            bool A1 = op.Egalite(op.Somme(x1, op.Somme(x2, x3)), op.Somme(op.Somme(x1, x2), x3));
            //Element neutre
            bool A2 = op.Egalite(op.Somme(x1, op.Zero()), op.Somme(op.Zero(), x1)) && op.Egalite(op.Somme(x1, op.Zero()), x1);
            //Symetrique
            bool A3 = op.Egalite(op.Somme(x1, op.Symetrique(x1)), op.Somme(op.Symetrique(x1), x1)) && op.Egalite(op.Somme(x1, op.Symetrique(x1)), op.Zero());
            return A1 && A2 && A3;
        }
        public bool TesterAxiomes(IOpGroupeAbelien<C> op, C x1, C x2, C x3)
        {
            //herite de groupe
            bool A1 = TesterAxiomes((IOpGroupe<C>)op, x1, x2, x3);
            //Symetrie
            bool A2 = op.Egalite(op.Somme(x1, x2), op.Somme(x2, x1));
            return A1 && A2;
        }
        public bool TesterAxiomes(IOpAnneauUnitaire<C> op, C x1, C x2, C x3)
        {
            //herite de groupe abélien
            bool A1 = TesterAxiomes((IOpGroupeAbelien<C>)op, x1, x2, x3);
            //associativité
            bool A2 = op.Egalite(op.Produit(x1, op.Produit(x2, x3)), op.Produit(op.Produit(x1, x2), x3));
            //distributivité
            bool A3 = op.Egalite(op.Produit(x1,op.Somme(x2,x3)),op.Somme(op.Produit(x1,x2),op.Produit(x1,x3)));
            bool A4 = op.Egalite(op.Produit( op.Somme(x2, x3),x1), op.Somme(op.Produit(x2, x1), op.Produit(x3, x1)));
            //Elt neutre
            bool A5 = op.Egalite(op.Produit(x1, op.Un()), op.Produit(op.Un(), x1)) && op.Egalite(op.Produit(x1, op.Un()), x1);
            return A1 && A2 && A3 && A4 && A5;
        }
        public bool TesterAxiomes(IOpAnneauCommutatif<C> op, C x1, C x2, C x3)
        {
            //herite de anneau unitaire
            bool A1 = TesterAxiomes((IOpAnneauUnitaire<C>) op, x1, x2, x3);
            //Commutativite
            bool A2 = op.Egalite(op.Produit(x1, x2), op.Produit(x2, x1));
            return A1 && A2;
        }
        public bool TesterAxiomes(IOpCorps<C> op, C x1, C x2, C x3)
        {
            bool A1 = TesterAxiomes((IOpAnneauCommutatif<C>)op, x1, x2, x3);
            bool A2 = true;
            if (op.NonNul(x1))
            {
                A2 = op.Egalite(op.Un(), op.Produit(x1, op.Inverse(x1)));
            }
            return A1 && A2;
        }
    }
    public struct TesteurAxiomes<E,K>
    {
        public bool TesterAxiomes(IOpEspaceVect<E,K> op, E e1, E e2, E e3, K k1, K k2, K k3)
        {
            TesteurAxiomes<E> TestAxE = new TesteurAxiomes<E>();
            TesteurAxiomes<K> TestAxK = new TesteurAxiomes<K>();
            //E est un groupe abelien
            bool A1 = TestAxE.TesterAxiomes(op.OpE(), e1, e2, e3);
            //K est un corps
            bool A2 = TestAxK.TesterAxiomes(op.OpK(), k1, k2, k3);
            //Loi conjointe
            //Distributivite
            bool A3 = op.OpE().Egalite(op.Point(k1, op.OpE().Somme(e1, e2)), op.OpE().Somme(op.Point(k1, e1), op.Point(k1, e2)));
            bool A4 = op.OpE().Egalite(op.Point(op.OpK().Somme(k1,k2),e1),op.OpE().Somme(op.Point(k1,e1),op.Point(k2,e1)));
            //associativité mixte
            bool A5 = op.OpE().Egalite(op.Point(op.OpK().Produit(k1,k2),e1),op.Point(k1,op.Point(k2,e1)));
            bool A6 = op.OpE().Egalite(op.Point(op.OpK().Un(), e1), e1);
            return A1 && A2 && A3 && A4 && A5 && A6;
        }
        public bool TesterAxiomes(IOpAlgebre<E, K> op, E e1, E e2, E e3, K k1, K k2, K k3)
        {
            //herite de espace vect
            bool A1 = TesterAxiomes((IOpEspaceVect<E, K>)op, e1, e2, e3, k1, k2, k3);
            //bilinéarité
            bool A2 = op.OpE().Egalite(op.Fois(op.OpE().Somme(e1,e2),e3),op.OpE().Somme(op.Fois(e1,e3),op.Fois(e2,e3)));
            bool A3 = op.OpE().Egalite(op.Fois(e1,op.OpE().Somme(e2, e3)), op.OpE().Somme(op.Fois(e1, e2), op.Fois(e1, e3)));
            bool A4 = op.OpE().Egalite(op.Fois(op.Point(k1,e1),op.Point(k2,e2)),op.Point(op.OpK().Produit(k1,k2),op.Fois(e1,e2)));
            return A1 && A2 && A3 && A4;
        }
        public bool TesterAxiomes(IOpEspacePreHilbertien<E, K> op, E e1, E e2, E e3, K k1, K k2, K k3)
        {
            //herite de espace vect
            bool A1 = TesterAxiomes((IOpEspaceVect<E, K>)op, e1, e2, e3, k1, k2, k3);
            //Symetrique
            bool A2 = op.OpK().Egalite(op.Scalaire(e1, e2), op.Scalaire(e2, e1));
            //Definie
            bool A3 = (!(op.OpK().Egalite(op.Scalaire(e1,e1),op.OpK().Zero()))) || (op.OpE().Egalite(e1, op.OpE().Zero()));
            //Positive
            bool A4 = (op.SupZero(op.Scalaire(e1, e1))) || (op.OpK().Egalite(op.Scalaire(e1, e1), op.OpK().Zero()));
            return A1 && A2 && A3 && A4;
        }
    }

    //Classes
    public class Matrice<T>
    {
        private T[,] Valeurs;
        public int m;
        public int n;

        public Matrice( int m, int n)
        {
            this.m = m;
            this.n = n;
            Valeurs = new T[m, n];
        }

        public T this[int i, int j]
        {
            get { return Valeurs[i, j]; }
            set { Valeurs[i, j] = value; }
        }

    }

    //Implémentations de classes
    public class OpReelCanonique : IOpCorps<double>
    {
        public bool Egalite(double a, double b)
        {
            return Math.Abs(a-b)<1e-15;
        }

        public double Inverse(double a)
        {
            return 1.0/a;
        }

        public bool NonNul(double a)
        {
            return a!=0.0;
        }

        public double Produit(double a, double b)
        {
            return a*b;
        }

        public double Somme(double a, double b)
        {
            return a+b;
        }

        public double Symetrique(double a)
        {
            return -a;
        }

        public double Un()
        {
            return 1.0;
        }

        public double Zero()
        {
            return 0.0;
        }
    }
    //Permet d'opérér sur les matrices contenant T, avec le corps des scalaires K, dont les interactions sont régies par l'opérateur OM
    public class OpGrpeAbMatriceCanonique<T> : IOpGroupeAbelien<Matrice<T>>
    {
        IOpGroupeAbelien<T> opElt;

        public OpGrpeAbMatriceCanonique(IOpGroupeAbelien<T> opElt)
        {
            this.opElt = opElt;
        }

        public bool Egalite(Matrice<T> a, Matrice<T> b)
        {
            throw new NotImplementedException();
        }

        public Matrice<T> Somme(Matrice<T> a, Matrice<T> b)
        {
            throw new NotImplementedException();
        }

        public Matrice<T> Symetrique(Matrice<T> a)
        {
            throw new NotImplementedException();
        }

        public Matrice<T> Zero()
        {
            throw new NotImplementedException();
        }
    }
    public class OpAlgMatriceCanonique<T, K> : IOpAlgebre<Matrice<T>, K>
    {
        private IOpAlgebre<T, K> OpMixte;

        public OpAlgMatriceCanonique(IOpAlgebre<T, K> opMixte)
        {
            OpMixte = opMixte;
        }

        public Matrice<T> Fois(Matrice<T> a, Matrice<T> b)
        {
            throw new NotImplementedException();
        }

        public IOpGroupeAbelien<Matrice<T>> OpE()
        {
            return new OpGrpeAbMatriceCanonique<T>(OpMixte.OpE());
        }

        public IOpCorps<K> OpK()
        {
            return OpMixte.OpK();
        }

        public Matrice<T> Point(K k, Matrice<T> e)
        {
            Matrice<T> ret = new Matrice<T>(e.m, e.n);
            for (int i=0;i<e.m;i++)
            {
                for (int j = 0; j < e.n; j++)
                {
                    ret[i, j] = OpMixte.Point(k, e[i, j]);
                }
            }
            return ret;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            OpReelCanonique op = new OpReelCanonique();
            Random r = new Random(100);
            TesteurAxiomes<double> axdb = new TesteurAxiomes<double>();
            for(int i=0;i<100;i++)
            {
                double a = r.NextDouble();
                double b = r.NextDouble();
                double c = r.NextDouble();
                Console.WriteLine(axdb.TesterAxiomes(op, a, b, c));
            }
        }
    }
}
