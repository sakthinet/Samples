package hello;
 
import javax.swing.SwingUtilities;
import java.awt.BorderLayout;
import javax.swing.JPanel;
import javax.swing.JFrame;
 
public class HelloJFrame extends JFrame {
 
    private static final long serialVersionUID = 1L;
 
    private JPanel jContentPane = null;
 
    /**
     * @param args
     */
    public static void main(String[] args) {
 
        SwingUtilities.invokeLater(new Runnable() {
            public void run() {
                HelloJFrame thisClass = new HelloJFrame();
                thisClass.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
                thisClass.setVisible(true);
            }
        });
    }
 
    /**
     * This is the default constructor
     */
    public HelloJFrame() {
        super();
        initialize();
    }
 
    /**
     * This method initializes this
     * 
     * @return void
     */
    private void initialize() {
        this.setSize(300, 200);
        this.setContentPane(getJContentPane());
        this.setTitle("JFrame");
    }
 
    /**
     * This method initializes jContentPane
     * 
     * @return javax.swing.JPanel
     */
    private JPanel getJContentPane() {
        if (jContentPane == null) {
            jContentPane = new JPanel();
            jContentPane.setLayout(new BorderLayout());
        }
        return jContentPane;
    }
 
}