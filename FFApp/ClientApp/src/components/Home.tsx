import * as React from 'react';
import { connect } from 'react-redux';

const Home = () => (
  <div>
    <h1>Financial Forecasting App</h1>
    <p>Welcome!</p>
  </div>
);

export default connect()(Home);
