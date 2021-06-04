import * as React from 'react';
//import { connect } from 'react-redux';
//import { RouteComponentProps } from 'react-router';
//import { Link } from 'react-router-dom';
//import { ApplicationState } from '../store';
//import * as InvestmentsStore from '../store/Investments';

export interface Investment {
    id: string;
    label: string;
    value: number;
    portfolioName: string;
}

interface ProjectionInvestmentState {
    id: string;
    days: number;
    expectedReturn: number;
    investments: Investment[];
    currentValue: string | undefined;
    projectedValue: string
}

class Projection extends React.Component<any, ProjectionInvestmentState> {
    constructor(props: any) {
        super(props);

        this.state =  {
            id: '',
            days: 0,
            expectedReturn: 0,
            investments: [],
            currentValue: '',
            projectedValue: ''
        };
    }

    public componentDidMount() {
        try {
            fetch(`api/investment/list`, {
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                }
            })
                .then(response => response.json() as Promise<Investment[]>)
                .then(data => this.setState({ investments: data }));
        } catch (err) {
            console.log(err);
        }
    }

    //isFormValid = () => {
    //    const { id, days, expectedReturn } = this.state;
    //    const isFormValid = id !== '' && days > 0 && expectedReturn !== 0;
    //}

    private onSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        fetch(`api/investment/compute`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({ id: this.state.id, days: this.state.days, expectedReturn: this.state.expectedReturn })
        })
            .then(response => response.json() as Promise<string>)
            .then(data => this.setState({ projectedValue: data }));
    };

    //private onChangeInvestment(event: React.ChangeEvent<HTMLSelectElement>) {
    //    var investment = this.state.investments.find(i => i.id === event.target.value);
    //    console.log(investment);

    //    this.setState({ id: event.target.value, projectedValue: '', currentValue: ''/*investment === undefined ? '' : investment.value.toString()*/ });
    //}

    //private onChangeExpectedReturn(event: React.ChangeEvent<HTMLInputElement>) {
    //    this.setState({ expectedReturn: parseFloat(event.target.value), projectedValue: '' });
    //}

    //private onChangeDays(event: React.ChangeEvent<HTMLInputElement>) {
    //    this.setState({ days: parseInt(event.target.value), projectedValue: '' });
    //}

    public render() {
        const { id, days, expectedReturn } = this.state;
        const isFormValid = id !== '' && days > 0 && expectedReturn !== 0;

        return (


            <div>
                <form onSubmit={this.onSubmit}>
                    <label>Investment:
                        <select value={this.state.id} onChange={(e) => this.setState({ id: e.target.value, projectedValue: '' })}>
                            <option value='' selected>Select an investisment</option>
                            {this.state.investments.map(investment => <option key={investment.id} value={investment.id}>{investment.label} ({investment.value})</option>)}
                        </select>                    
                    </label>
                    <br/>
                    <label>Number of Days:
                        <input value={this.state.days} onChange={(e) => this.setState({ days: parseInt(e.target.value), projectedValue: '' })} />
                    </label>
                    <br />

                    <label>Expected Annual Return:
                        <input value={this.state.expectedReturn} onChange={(e) => this.setState({ expectedReturn: parseFloat(e.target.value), projectedValue: '' })} />
                    </label>
                    <br />
                    <button disabled={!isFormValid}>Calculate</button>
                </form>

                <p aria-live="polite">Projected Value: <strong>{this.state.projectedValue}</strong></p>
            </div>
        );
    }

}

export default Projection;
