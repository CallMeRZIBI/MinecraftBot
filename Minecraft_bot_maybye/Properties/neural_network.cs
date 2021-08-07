using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_bot_maybye
{
    class neural_network
    {
        private int input_nodes;
        private int hidden_nodes;
        private int output_nodes;
        private double lr;
        private Matrix weights_inh;
        private Matrix weights_hio;
        private Matrix bias_hi;
        private Matrix bias_ou;

        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public static double Dsigmoid(double y)
        {
            //return Sigmoid(x) * (1 - Sigmoid(x));
            return y * (1 - y);
        }

        public neural_network(int numI, int numH, int numO)
        {
            this.input_nodes = numI;
            this.hidden_nodes = numH;
            this.output_nodes = numO;
            this.lr = 0.0001;

            Matrix weights_ih = new Matrix(this.hidden_nodes, this.input_nodes);
            weights_inh = weights_ih;
            Matrix weights_ho = new Matrix(this.output_nodes, this.hidden_nodes);
            weights_hio = weights_ho;
            weights_inh.Random();
            weights_hio.Random();

            Matrix bias_h = new Matrix(this.hidden_nodes, 1);
            bias_hi = bias_h;
            bias_hi.Random();
            Matrix bias_o = new Matrix(this.output_nodes, 1);
            bias_ou = bias_o;
            bias_ou.Random();
        }

        public Matrix feedforward(double[] input_array)
        {
            //lots of matrix
            //Generating the hidden outputs
            var inputs = Matrix.FromArray(input_array);
            var hidden = Matrix.Multiply(this.weights_inh, inputs);
            hidden.Add(this.bias_hi);
            //activation function
            hidden.Map(Sigmoid);

            //generating the output
            var output = Matrix.Multiply(this.weights_hio, hidden);
            output.Add(this.bias_ou);
            output.Map(Sigmoid);

            //sending back to caller
            return output;
        }

        //change from void to matrix
        public void Train(double[] inputs, double[] targets_array)
        {
            //lots of matrix
            //Generating the hidden outputs
            var inputs_ = Matrix.FromArray(inputs);
            var hidden = Matrix.Multiply(this.weights_inh, inputs_);
            hidden.Add(this.bias_hi);
            //activation function
            hidden.Map(Sigmoid);

            //generating the output
            var outputs = Matrix.Multiply(this.weights_hio, hidden);
            outputs.Add(this.bias_ou);
            outputs.Map(Sigmoid);

            //convert to matrix
            var targets = Matrix.FromArray(targets_array);
            //calculate the error
            //error = targets - ouputs
            var outputErrors = Matrix.Subtract(targets, outputs);

            //calculate gradient
            var gradient = Matrix.Map(outputs, Dsigmoid);
            gradient = Matrix.Multiply(gradient, outputErrors);
            //gradient is somehow null
            gradient.Multiply(this.lr);

            //calculate deltas
            var HiddenT = Matrix.Transpose(hidden);
            var weight_ho_delta = Matrix.Multiply(gradient, HiddenT);

            //adjust the weights by deltas
            this.weights_hio.Add(weight_ho_delta);
            //adjust the biases by its deltas (which are just gradients)
            this.bias_ou.Add(gradient);

            //calculate the hidden layer errors
            var who_t = Matrix.Transpose(this.weights_hio);
            var hiddenErrors = Matrix.Multiply(who_t, outputErrors);

            //Calculate hidden gradient
            var hidden_gradient = Matrix.Map(hidden, Dsigmoid);
            hidden_gradient = Matrix.Multiply(hidden_gradient, hiddenErrors);
            //hidden_gradient is somehow null
            hidden_gradient.Multiply(this.lr);

            //Calculate input->hidden deltas
            var inputT = Matrix.Transpose(inputs_);
            var weight_ih_deltas = Matrix.Multiply(hidden_gradient, inputT);

            this.weights_inh.Add(weight_ih_deltas);
            this.bias_hi.Add(hidden_gradient);
        }
    }
}
