import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { BoardList } from './components/BoardList';
import { BoardReader } from './components/BoardReader';
import { AllBoardReader } from './components/AllBoardReader';
import { StatisticsList } from './components/StatisticsList';
import { StatisticsBoard } from './components/StatisticsBoard';
import { Login } from './components/Login';
import { PresentationList } from './components/PresentationList';

export const routes = <Layout>
    <Route exact path='/' component={AllBoardReader} />
    <Route path='/login' component={Login} />
    <Route exact path='/admin' component={PresentationList} />
    <Route path='/admin/statistics' component={StatisticsList} />
    <Route path="/admin/jiraconnectionstats/:id" component={StatisticsBoard} />
    <Route path='/admin/presentations' component={PresentationList} />
    <Route path='/admin/createPresentation' component={BoardList} />
    <Route path='/admin/presentations/:id' component={BoardList} />
</Layout>;