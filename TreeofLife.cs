namespace Tree_of_Life
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Initialize the model
            Modele modele = new Modele();
            // Get the root node
            Modele.Node rootNode = modele.getRootNode();
            // Display the root node's name
            //MessageBox.Show("Root Node: " + rootNode.getName());

            // Initialize the screen
            Ecran.ZoneArbre arbre = new Ecran.ZoneArbre();
            this.Controls.Add(arbre);
            // Set the form's properties
            //this.Text = "Tree of Life";
            Ecran.ZoneMenu menu = new Ecran.ZoneMenu();
            this.Controls.Add(menu);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
