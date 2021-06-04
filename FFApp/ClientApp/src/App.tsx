import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import InvestmentBreakdown from './components/InvestmentBreakdown';
import Projection from './components/Projection';

import './custom.css'

export default () => (
    <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/projection' component={Projection} />
        <Route path='/investmentbreakdown' component={InvestmentBreakdown} />
    </Layout>
);
